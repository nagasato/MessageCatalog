# MessageCatalog

TSVファイルからタイプセーフなメッセージカタログを自動生成するC# Source Generatorです。

[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/nagasato/MessageCatalog)


## 特徴

- **タイプセーフ**: TSVファイルで定義したメッセージIDがプロパティとして生成され、コンパイル時にチェックされます
- **IntelliSense対応**: XMLドキュメントコメントが自動生成され、IDEでメッセージ内容を確認できます
- **複数カタログ対応**: 複数のTSVファイルから異なるメッセージカタログを生成可能
- **実行時テキスト上書き**: 実行時にTSVファイルを読み込んでメッセージテキストを上書き可能
- **.NET Framework / .NET対応**: .NET Framework 4.8 および .NET 8 の両方で動作

## インストール

### プロジェクト参照

```xml
<ItemGroup>
  <ProjectReference Include="path\to\MessageCatalog.SourceGenerator.csproj" 
                    ReferenceOutputAssembly="false" 
                    OutputItemType="Analyzer" />
</ItemGroup>
```

## 使い方

### 1. TSVファイルを作成

プロジェクトに `messages.tsv` ファイルを作成します：

```tsv
Id	Text	Category	Severity	Description
INFO001	処理が正常に完了しました。	None	Information	
INFO002	ユーザー '{0}' の登録が完了しました。	User	Information	ユーザー登録時の成功メッセージ
WARN001	ファイル '{0}' が見つかりません。	File	Warning	ファイル読込時のエラーメッセージ
ERR001	入力値が不正です: {0}	Input	Error	入力エラー時のメッセージ
FATAL001	システムエラーが発生しました。エラーコード: {0}	System	Fatal	回復不能エラーのメッセージ
```

<details>
<summary>テーブル形式で表示</summary>

| Id | Text | Category | Severity | Description |
|----|------|----------|----------|-------------|
| INFO001 | 処理が正常に完了しました。 | None | Information | |
| INFO002 | ユーザー '{0}' の登録が完了しました。 | User | Information | ユーザー登録時の成功メッセージ |
| WARN001 | ファイル '{0}' が見つかりません。 | File | Warning | ファイル読込時のエラーメッセージ |
| ERR001 | 入力値が不正です: {0} | Input | Error | 入力エラー時のメッセージ |
| FATAL001 | システムエラーが発生しました。エラーコード: {0} | System | Fatal | 回復不能エラーのメッセージ |

</details>

### 2. プロジェクトファイルを設定

TSVファイルをAdditionalFilesとして追加します：

```xml
<ItemGroup>
  <AdditionalFiles Include="messages.tsv" />
</ItemGroup>
```

### 3. コードで使用

```csharp
using MessageCatalog.Core;

// メッセージカタログをインスタンス化
var messageCatalog = new DefaultMessageCatalog();

// メッセージを使用
Console.WriteLine(messageCatalog.INFO001.Text);
// 出力: 処理が正常に完了しました。

// フォーマット付きメッセージ
Console.WriteLine(messageCatalog.INFO002.Format("James"));
// 出力: ユーザー 'James' の登録が完了しました。

// メタデータにアクセス
Console.WriteLine($"Category: {messageCatalog.ERR001.Category}");
Console.WriteLine($"Severity: {messageCatalog.ERR001.Servity}");
Console.WriteLine($"Description: {messageCatalog.ERR001.Description}");
```

## TSVファイル形式

| カラム | 必須 | 説明 |
|--------|------|------|
| Id | :heavy_check_mark: | メッセージの一意識別子（プロパティ名になります） |
| Text | :heavy_check_mark: | メッセージテキスト（`{0}`, `{1}` などのプレースホルダー使用可） |
| Category | | メッセージのカテゴリ（enum として生成されます） |
| Severity | | メッセージの重要度（enum として生成されます） |
| Description | | 開発者向けの説明（XMLドキュメントに含まれます） |

## 複数のメッセージカタログ

ファイル名を `Prefix_messages.tsv` の形式にすると、別のカタログクラスが生成されます：

| ファイル名 | 生成されるクラス |
|------------|------------------|
| `messages.tsv` | `DefaultMessageCatalog`, `DefaultMessageItem` |
| `Validation_messages.tsv` | `ValidationMessageCatalog`, `ValidationMessageItem` |
| `Error_messages.tsv` | `ErrorMessageCatalog`, `ErrorMessageItem` |

```csharp
var defaultCatalog = new DefaultMessageCatalog();
var validationCatalog = new ValidationMessageCatalog();

Console.WriteLine(defaultCatalog.INFO001.Text);
Console.WriteLine(validationCatalog.VALID001.Text);
```

## 実行時テキスト上書き

コンストラクタにTSVファイルのパスを指定することで、実行時にメッセージテキストを上書きできます。これにより、再コンパイルなしでメッセージを変更できます。

```csharp
// 実行時にTSVファイルからテキストを読み込む
var catalog = new DefaultMessageCatalog("custom_messages.tsv");
```

## 詳細な使用例

### 基本的な使い方

```csharp
using MessageCatalog.Core;

// メッセージカタログをインスタンス化
var messageCatalog = new DefaultMessageCatalog();

// シンプルなメッセージの取得
Console.WriteLine(messageCatalog.INFO001.Text);
// 出力: 処理が正常に完了しました。

// プレースホルダー付きメッセージのフォーマット
Console.WriteLine(messageCatalog.INFO002.Format("James"));
// 出力: ユーザー 'James' の登録が完了しました。

Console.WriteLine(messageCatalog.WARN001.Format("config.json"));
// 出力: ファイル 'config.json' が見つかりません。

Console.WriteLine(messageCatalog.ERR001.Format("無効な値"));
// 出力: 入力値が不正です: 無効な値

Console.WriteLine(messageCatalog.FATAL001.Format("E-005-01"));
// 出力: システムエラーが発生しました。エラーコード: E-005-01
```

### メタデータの活用

```csharp
var messageCatalog = new DefaultMessageCatalog();
var message = messageCatalog.ERR001;

// メッセージのメタデータにアクセス
Console.WriteLine($"ID: ERR001");
Console.WriteLine($"Text: {message.Text}");
Console.WriteLine($"Category: {message.Category}");      // 出力: Input
Console.WriteLine($"Severity: {message.Servity}");       // 出力: Error
Console.WriteLine($"Description: {message.Description}"); // 出力: 入力エラー時のメッセージ

// Severityに基づいた条件分岐
if (message.Servity == DefaultMessageServity.Error || 
    message.Servity == DefaultMessageServity.Fatal)
{
    // エラーログに出力
    Console.Error.WriteLine(message.Format("invalid input"));
}
```

### 複数カタログの併用

```csharp
using MessageCatalog.Core;

// 複数のメッセージカタログを使用
var messageCatalog = new DefaultMessageCatalog();
var validationCatalog = new ValidationMessageCatalog();

Console.WriteLine("=== DefaultMessageCatalog ===");
Console.WriteLine($"{messageCatalog.INFO001.Text}");
Console.WriteLine($"{messageCatalog.INFO002.Format("James")}");
Console.WriteLine($"{messageCatalog.WARN001.Format("file.txt")}");
Console.WriteLine($"{messageCatalog.ERR001.Format("FooBar")}");
Console.WriteLine($"{messageCatalog.FATAL001.Format("E-005-01")}");

Console.WriteLine();

Console.WriteLine("=== ValidationMessageCatalog ===");
Console.WriteLine($"{validationCatalog.VALID001.Text}");
Console.WriteLine($"{validationCatalog.VALID002.Format("ユーザー名")}");
Console.WriteLine($"{validationCatalog.VALID003.Format("メールアドレス")}");
```

### DIコンテナとの統合

```csharp
using MessageCatalog.Core;
using Microsoft.Extensions.DependencyInjection;

// サービスの登録
var services = new ServiceCollection();
services.AddSingleton<DefaultMessageCatalog>();
services.AddSingleton<ValidationMessageCatalog>();
services.AddTransient<Application>();

var provider = services.BuildServiceProvider();
var app = provider.GetRequiredService<Application>();
app.Run();

// アプリケーションクラス
public class Application
{
    private readonly DefaultMessageCatalog _messageCatalog;
    private readonly ValidationMessageCatalog _validationCatalog;

    public Application(
        DefaultMessageCatalog messageCatalog, 
        ValidationMessageCatalog validationCatalog)
    {
        _messageCatalog = messageCatalog;
        _validationCatalog = validationCatalog;
    }

    public void Run()
    {
        Console.WriteLine(_messageCatalog.INFO001.Text);
        Console.WriteLine(_validationCatalog.VALID001.Text);
    }
}
```

### ログ出力との連携

```csharp
using MessageCatalog.Core;
using Microsoft.Extensions.Logging;

public class OrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly DefaultMessageCatalog _messages;

    public OrderService(ILogger<OrderService> logger, DefaultMessageCatalog messages)
    {
        _logger = logger;
        _messages = messages;
    }

    public void ProcessOrder(string orderId)
    {
        // Severityに応じたログレベルで出力
        var infoMessage = _messages.INFO001;
        _logger.LogInformation("{Message}", infoMessage.Text);

        var warnMessage = _messages.WARN001;
        _logger.LogWarning("{Message}", warnMessage.Format("order.json"));

        var errorMessage = _messages.ERR001;
        _logger.LogError("{Message}", errorMessage.Format(orderId));
    }
}
```

## 生成されるコード

### メッセージアイテムクラス

```csharp
public class DefaultMessageItem
{
    public string Text { get; }
    public DefaultMessageCategory Category { get; }
    public DefaultMessageServity Servity { get; }
    public string Description { get; }
    
    public string Format(params object[] args);
}
```

### カテゴリとSeverityのEnum

TSVファイルで使用されているCategory/Severityの値から自動的にenumが生成されます：

```csharp
public enum DefaultMessageCategory { None, User, File, Input, System, ... }
public enum DefaultMessageServity { None, Information, Warning, Error, Fatal, ... }
```

## 対応環境

- .NET 8.0 以降
- .NET Framework 4.8
- .NET Standard 2.0（Source Generator自体）

