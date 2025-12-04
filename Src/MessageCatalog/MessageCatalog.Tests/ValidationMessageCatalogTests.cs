using global::MessageCatalog.Core;

namespace MessageCatalog.Tests;

public class ValidationMessageCatalogTests
{
    private readonly ValidationMessageCatalog _validationMessageCatalog;

    public ValidationMessageCatalogTests()
    {
        _validationMessageCatalog = new ValidationMessageCatalog("TestData/Validation_messages.json");
    }

    [Fact]
    public void ValidationMessageCatalog_ShouldHaveMessages()
    {
        // Arrange & Act & Assert
        Assert.NotNull(_validationMessageCatalog.VALID001);
    }

    [Fact]
    public void VALID001_ShouldHaveCorrectProperties()
    {
        // Arrange
        var message = _validationMessageCatalog.VALID001;

        // Assert
        Assert.Equal("入力値が正しく検証されました。", message.Text);
        Assert.Equal(ValidationMessageCategory.Validation, message.Category);
        Assert.Equal(ValidationMessageServity.Information, message.Servity);
        Assert.Equal("検証成功時のメッセージ", message.Description);
    }

    [Fact]
    public void ValidationMessageItem_ToString_ShouldReturnText()
    {
        // Arrange
        var message = _validationMessageCatalog.VALID001;

        // Act
        var result = message.ToString();

        // Assert
        Assert.Equal("入力値が正しく検証されました。", result);
    }
}
