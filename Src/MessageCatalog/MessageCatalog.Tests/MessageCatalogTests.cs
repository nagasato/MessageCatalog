using MessageCatalog.Core;

namespace MessageCatalog.Tests;

public class MessageCatalogTests
{
    private readonly DefaultMessageCatalog _messageCatalog;

    public MessageCatalogTests()
    {
        _messageCatalog = new DefaultMessageCatalog("TestData/messages.tsv");
    }

    [Fact]
    public void MessageCatalog_ShouldHaveAllMessages()
    {
        // Arrange & Act & Assert
        Assert.NotNull(_messageCatalog.TEST001);
        Assert.NotNull(_messageCatalog.TEST002);
        Assert.NotNull(_messageCatalog.TEST003);
    }

    [Fact]
    public void TEST001_ShouldHaveCorrectProperties()
    {
        // Arrange
        var message = _messageCatalog.TEST001;

        // Assert
        Assert.Equal("テストメッセージ1", message.Text);
        Assert.Equal(DefaultMessageCategory.Test, message.Category);
        Assert.Equal(DefaultMessageServity.Information, message.Servity);
        Assert.Equal("テスト用メッセージ1", message.Description);
    }

    [Fact]
    public void TEST002_ShouldFormatWithParameter()
    {
        // Arrange
        var message = _messageCatalog.TEST002;

        // Act
        var formatted = message.Format("テスト値");

        // Assert
        Assert.Equal("パラメーター 'テスト値' のテスト", formatted);
        Assert.Equal(DefaultMessageCategory.Test, message.Category);
        Assert.Equal(DefaultMessageServity.Warning, message.Servity);
    }

    [Fact]
    public void TEST003_ShouldHaveEmptyDescription()
    {
        // Arrange
        var message = _messageCatalog.TEST003;

        // Assert
        Assert.Equal("エラーテスト", message.Text);
        Assert.Equal(DefaultMessageCategory.Test, message.Category);
        Assert.Equal(DefaultMessageServity.Error, message.Servity);
        Assert.Equal(string.Empty, message.Description);
    }

    [Fact]
    public void MessageItem_ToString_ShouldReturnText()
    {
        // Arrange
        var message = _messageCatalog.TEST001;

        // Act
        var result = message.ToString();

        // Assert
        Assert.Equal("テストメッセージ1", result);
    }

    [Fact]
    public void MessageItem_Format_WithMultipleParameters_ShouldWork()
    {
        // Arrange
        var message = _messageCatalog.TEST002;

        // Act
        var formatted = message.Format("値1");

        // Assert
        Assert.Equal("パラメーター '値1' のテスト", formatted);
    }

    [Theory]
    [InlineData("TEST001", DefaultMessageServity.Information)]
    [InlineData("TEST002", DefaultMessageServity.Warning)]
    [InlineData("TEST003", DefaultMessageServity.Error)]
    public void Messages_ShouldHaveCorrectServity(string messageId, DefaultMessageServity expectedServity)
    {
        // Arrange & Act
        var message = messageId switch
        {
            "TEST001" => _messageCatalog.TEST001,
            "TEST002" => _messageCatalog.TEST002,
            "TEST003" => _messageCatalog.TEST003,
            _ => throw new ArgumentException("Invalid message ID")
        };

        // Assert
        Assert.Equal(expectedServity, message.Servity);
    }
}
