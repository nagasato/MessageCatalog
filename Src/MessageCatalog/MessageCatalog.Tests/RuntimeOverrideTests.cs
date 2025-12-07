using MessageCatalog.Core;

namespace MessageCatalog.Tests;

/// <summary>
/// Tests for runtime message override with Japanese TSV files
/// </summary>
public class RuntimeOverrideTests
{
    [Fact]
    public void DefaultMessageCatalog_WithJapaneseTsv_ShouldOverrideText()
    {
        // Arrange
        var catalog = new DefaultMessageCatalog("TestData/messages_ja.tsv");

        // Act & Assert
        Assert.Equal("テストメッセージ1", catalog.TEST001.Text);
    }

    [Fact]
    public void DefaultMessageCatalog_WithJapaneseTsv_ShouldFormatCorrectly()
    {
        // Arrange
        var catalog = new DefaultMessageCatalog("TestData/messages_ja.tsv");

        // Act
        var formatted = catalog.TEST002.Format("テスト値");

        // Assert
        Assert.Equal("パラメーター 'テスト値' のテスト", formatted);
    }

    [Fact]
    public void DefaultMessageCatalog_WithJapaneseTsv_ShouldReturnJapaneseForAllMessages()
    {
        // Arrange
        var catalog = new DefaultMessageCatalog("TestData/messages_ja.tsv");

        // Act & Assert
        Assert.Equal("テストメッセージ1", catalog.TEST001.Text);
        Assert.Equal("パラメーター '{0}' のテスト", catalog.TEST002.Text);
        Assert.Equal("エラーテスト", catalog.TEST003.Text);
    }

    [Fact]
    public void DefaultMessageCatalog_WithJapaneseTsv_ToString_ShouldReturnJapaneseText()
    {
        // Arrange
        var catalog = new DefaultMessageCatalog("TestData/messages_ja.tsv");

        // Act
        var result = catalog.TEST001.ToString();

        // Assert
        Assert.Equal("テストメッセージ1", result);
    }

    [Fact]
    public void ValidationMessageCatalog_WithJapaneseTsv_ShouldOverrideText()
    {
        // Arrange
        var catalog = new ValidationMessageCatalog("TestData/Validation_messages_ja.tsv");

        // Act & Assert
        Assert.Equal("入力値が正しく検証されました。", catalog.VALID001.Text);
    }

    [Fact]
    public void ValidationMessageCatalog_WithJapaneseTsv_ShouldFormatCorrectly()
    {
        // Arrange
        var catalog = new ValidationMessageCatalog("TestData/Validation_messages_ja.tsv");

        // Act
        var formatted = catalog.VALID002.Format("ユーザー名");

        // Assert
        Assert.Equal("必須項目 'ユーザー名' が入力されていません。", formatted);
    }

    [Fact]
    public void ValidationMessageCatalog_WithJapaneseTsv_ShouldReturnJapaneseForAllMessages()
    {
        // Arrange
        var catalog = new ValidationMessageCatalog("TestData/Validation_messages_ja.tsv");

        // Act & Assert
        Assert.Equal("入力値が正しく検証されました。", catalog.VALID001.Text);
        Assert.Equal("必須項目 '{0}' が入力されていません。", catalog.VALID002.Text);
        Assert.Equal("'{0}' の形式が正しくありません。", catalog.VALID003.Text);
    }

    [Fact]
    public void DefaultMessageCatalog_CompareEnglishAndJapanese_ShouldHaveDifferentText()
    {
        // Arrange
        var englishCatalog = new DefaultMessageCatalog("TestData/messages.tsv");
        var japaneseCatalog = new DefaultMessageCatalog("TestData/messages_ja.tsv");

        // Act & Assert
        Assert.NotEqual(englishCatalog.TEST001.Text, japaneseCatalog.TEST001.Text);
        Assert.Equal("Test message 1", englishCatalog.TEST001.Text);
        Assert.Equal("テストメッセージ1", japaneseCatalog.TEST001.Text);
    }

    [Fact]
    public void ValidationMessageCatalog_CompareEnglishAndJapanese_ShouldHaveDifferentText()
    {
        // Arrange
        var englishCatalog = new ValidationMessageCatalog("TestData/Validation_messages.tsv");
        var japaneseCatalog = new ValidationMessageCatalog("TestData/Validation_messages_ja.tsv");

        // Act & Assert
        Assert.NotEqual(englishCatalog.VALID001.Text, japaneseCatalog.VALID001.Text);
        Assert.Equal("Input has been validated successfully.", englishCatalog.VALID001.Text);
        Assert.Equal("入力値が正しく検証されました。", japaneseCatalog.VALID001.Text);
    }

    [Fact]
    public void MessageCatalog_MetadataRemainsSame_RegardlessOfLanguage()
    {
        // Arrange
        var englishCatalog = new DefaultMessageCatalog("TestData/messages.tsv");
        var japaneseCatalog = new DefaultMessageCatalog("TestData/messages_ja.tsv");

        // Act & Assert - Category and Severity should be the same
        Assert.Equal(englishCatalog.TEST001.Category, japaneseCatalog.TEST001.Category);
        Assert.Equal(englishCatalog.TEST001.Severity, japaneseCatalog.TEST001.Severity);
        Assert.Equal(englishCatalog.TEST002.Category, japaneseCatalog.TEST002.Category);
        Assert.Equal(englishCatalog.TEST002.Severity, japaneseCatalog.TEST002.Severity);
    }
}
