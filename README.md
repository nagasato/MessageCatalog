# MessageCatalog

A C# Source Generator that automatically generates type-safe message catalogs from TSV files.

[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/nagasato/MessageCatalog)

[日本語版 README](README_ja.md)

## Features

- **Type-safe**: Message IDs defined in TSV files are generated as properties and checked at compile time
- **IntelliSense support**: XML documentation comments are auto-generated, allowing you to view message content in your IDE
- **Multiple catalog support**: Generate different message catalogs from multiple TSV files
- **Runtime text override**: Load TSV files at runtime to override message text
- **.NET Framework / .NET support**: Works with both .NET Framework 4.8 and .NET 8

## Installation

### Project Reference

```xml
<ItemGroup>
  <ProjectReference Include="path\to\MessageCatalog.SourceGenerator.csproj" 
                    ReferenceOutputAssembly="false" 
                    OutputItemType="Analyzer" />
</ItemGroup>
```

## Usage

### 1. Create a TSV File

Create a `messages.tsv` file in your project:

```tsv
Id	Text	Category	Severity	Description
INFO001	Process completed successfully.	None	Information	
INFO002	User '{0}' has been registered.	User	Information	Success message for user registration
WARN001	File '{0}' not found.	File	Warning	Error message when reading file
ERR001	Invalid input: {0}	Input	Error	Message for input errors
FATAL001	System error occurred. Error code: {0}	System	Fatal	Message for unrecoverable errors
```

<details>
<summary>View as table</summary>

| Id | Text | Category | Severity | Description |
|----|------|----------|----------|-------------|
| INFO001 | Process completed successfully. | None | Information | |
| INFO002 | User '{0}' has been registered. | User | Information | Success message for user registration |
| WARN001 | File '{0}' not found. | File | Warning | Error message when reading file |
| ERR001 | Invalid input: {0} | Input | Error | Message for input errors |
| FATAL001 | System error occurred. Error code: {0} | System | Fatal | Message for unrecoverable errors |

</details>

### 2. Configure Project File

Add the TSV file as AdditionalFiles:

```xml
<ItemGroup>
  <AdditionalFiles Include="messages.tsv" />
</ItemGroup>
```

### 3. Use in Code

```csharp
using MessageCatalog.Core;

// Instantiate the message catalog
var messageCatalog = new DefaultMessageCatalog();

// Use messages
Console.WriteLine(messageCatalog.INFO001.Text);
// Output: Process completed successfully.

// Format messages with placeholders
Console.WriteLine(messageCatalog.INFO002.Format("James"));
// Output: User 'James' has been registered.

// Access metadata
Console.WriteLine($"Category: {messageCatalog.ERR001.Category}");
Console.WriteLine($"Severity: {messageCatalog.ERR001.Severity}");
Console.WriteLine($"Description: {messageCatalog.ERR001.Description}");
```

## TSV File Format

| Column | Required | Description |
|--------|----------|-------------|
| Id | :heavy_check_mark: | Unique identifier for the message (becomes property name) |
| Text | :heavy_check_mark: | Message text (can use `{0}`, `{1}` placeholders) |
| Category | | Message category (generated as enum) |
| Severity | | Message severity (generated as enum) |
| Description | | Developer description (included in XML documentation) |

## Multiple Message Catalogs

Using the filename format `Prefix_messages.tsv` generates a separate catalog class:

| Filename | Generated Classes |
|----------|-------------------|
| `messages.tsv` | `DefaultMessageCatalog`, `DefaultMessageItem` |
| `Validation_messages.tsv` | `ValidationMessageCatalog`, `ValidationMessageItem` |
| `Error_messages.tsv` | `ErrorMessageCatalog`, `ErrorMessageItem` |

```csharp
var defaultCatalog = new DefaultMessageCatalog();
var validationCatalog = new ValidationMessageCatalog();

Console.WriteLine(defaultCatalog.INFO001.Text);
Console.WriteLine(validationCatalog.VALID001.Text);
```

## Runtime Text Override

You can override message text at runtime by specifying a TSV file path in the constructor. This allows you to change messages without recompiling.

```csharp
// Load text from TSV file at runtime
var catalog = new DefaultMessageCatalog("custom_messages.tsv");
```

### Localization Example

You can use this feature for localization by providing language-specific TSV files:

```csharp
// Default messages (English)
var catalog = new DefaultMessageCatalog();
Console.WriteLine(catalog.INFO001.Text);
// Output: Process completed successfully.

// Override with Japanese messages at runtime
var japaneseCatalog = new DefaultMessageCatalog("messages_ja.tsv");
Console.WriteLine(japaneseCatalog.INFO001.Text);
// Output: 処理が正常に完了しました。
```

## Detailed Usage Examples

### Basic Usage

```csharp
using MessageCatalog.Core;

// Instantiate the message catalog
var messageCatalog = new DefaultMessageCatalog();

// Get simple messages
Console.WriteLine(messageCatalog.INFO001.Text);
// Output: Process completed successfully.

// Format messages with placeholders
Console.WriteLine(messageCatalog.INFO002.Format("James"));
// Output: User 'James' has been registered.

Console.WriteLine(messageCatalog.WARN001.Format("config.json"));
// Output: File 'config.json' not found.

Console.WriteLine(messageCatalog.ERR001.Format("invalid value"));
// Output: Invalid input: invalid value

Console.WriteLine(messageCatalog.FATAL001.Format("E-005-01"));
// Output: System error occurred. Error code: E-005-01
```

### Using Metadata

```csharp
var messageCatalog = new DefaultMessageCatalog();
var message = messageCatalog.ERR001;

// Access message metadata
Console.WriteLine($"ID: ERR001");
Console.WriteLine($"Text: {message.Text}");
Console.WriteLine($"Category: {message.Category}");      // Output: Input
Console.WriteLine($"Severity: {message.Severity}");       // Output: Error
Console.WriteLine($"Description: {message.Description}"); // Output: Message for input errors

// Conditional branching based on Severity
if (message.Severity == DefaultMessageSeverity.Error || 
    message.Severity == DefaultMessageSeverity.Fatal)
{
    // Output to error log
    Console.Error.WriteLine(message.Format("invalid input"));
}
```

### Using Multiple Catalogs

```csharp
using MessageCatalog.Core;

// Use multiple message catalogs
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
Console.WriteLine($"{validationCatalog.VALID002.Format("username")}");
Console.WriteLine($"{validationCatalog.VALID003.Format("email")}");
```

### DI Container Integration

```csharp
using MessageCatalog.Core;
using Microsoft.Extensions.DependencyInjection;

// Register services
var services = new ServiceCollection();
services.AddSingleton<DefaultMessageCatalog>();
services.AddSingleton<ValidationMessageCatalog>();
services.AddTransient<Application>();

var provider = services.BuildServiceProvider();
var app = provider.GetRequiredService<Application>();
app.Run();

// Application class
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

### Logging Integration

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
        // Output with log level based on Severity
        var infoMessage = _messages.INFO001;
        _logger.LogInformation("{Message}", infoMessage.Text);

        var warnMessage = _messages.WARN001;
        _logger.LogWarning("{Message}", warnMessage.Format("order.json"));

        var errorMessage = _messages.ERR001;
        _logger.LogError("{Message}", errorMessage.Format(orderId));
    }
}
```

## Generated Code

### Message Item Class

```csharp
public class DefaultMessageItem
{
    public string Text { get; }
    public DefaultMessageCategory Category { get; }
    public DefaultMessageSeverity Severity { get; }
    public string Description { get; }
    
    public string Format(params object[] args);
}
```

### Category and Severity Enums

Enums are automatically generated from the Category/Severity values used in the TSV file:

```csharp
public enum DefaultMessageCategory { None, User, File, Input, System, ... }
public enum DefaultMessageSeverity { None, Information, Warning, Error, Fatal, ... }
```

## Supported Environments

- .NET 8.0 or later
- .NET Framework 4.8
- .NET Standard 2.0 (Source Generator itself)

