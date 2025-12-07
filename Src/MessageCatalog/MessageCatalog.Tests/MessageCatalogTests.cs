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
        Assert.Equal("Test message 1", message.Text);
        Assert.Equal(DefaultMessageCategory.Test, message.Category);
        Assert.Equal(DefaultMessageSeverity.Information, message.Severity);
        Assert.Equal("Test message for testing 1", message.Description);
    }

    [Fact]
    public void TEST002_ShouldFormatWithParameter()
    {
        // Arrange
        var message = _messageCatalog.TEST002;

        // Act
        var formatted = message.Format("testValue");

        // Assert
        Assert.Equal("Test for parameter 'testValue'", formatted);
        Assert.Equal(DefaultMessageCategory.Test, message.Category);
        Assert.Equal(DefaultMessageSeverity.Warning, message.Severity);
    }

    [Fact]
    public void TEST003_ShouldHaveEmptyDescription()
    {
        // Arrange
        var message = _messageCatalog.TEST003;

        // Assert
        Assert.Equal("Error test", message.Text);
        Assert.Equal(DefaultMessageCategory.Test, message.Category);
        Assert.Equal(DefaultMessageSeverity.Error, message.Severity);
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
        Assert.Equal("Test message 1", result);
    }

    [Fact]
    public void MessageItem_Format_WithMultipleParameters_ShouldWork()
    {
        // Arrange
        var message = _messageCatalog.TEST002;

        // Act
        var formatted = message.Format("value1");

        // Assert
        Assert.Equal("Test for parameter 'value1'", formatted);
    }

    [Theory]
    [InlineData("TEST001", DefaultMessageSeverity.Information)]
    [InlineData("TEST002", DefaultMessageSeverity.Warning)]
    [InlineData("TEST003", DefaultMessageSeverity.Error)]
    public void Messages_ShouldHaveCorrectSeverity(string messageId, DefaultMessageSeverity expectedSeverity)
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
        Assert.Equal(expectedSeverity, message.Severity);
    }
}
