using MessageCatalog.Core;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Instantiate message catalogs (default: English)
            var messageCatalog = new DefaultMessageCatalog();
            var validationMessageCatalog = new ValidationMessageCatalog();

            // Run the application with English messages
            Console.WriteLine("========================================");
            Console.WriteLine("  Default Messages (English)");
            Console.WriteLine("========================================");
            var app = new Application(messageCatalog, validationMessageCatalog);
            app.Run();

            Console.WriteLine();

            // Demonstrate runtime message override with Japanese
            Console.WriteLine("========================================");
            Console.WriteLine("  Runtime Override (Japanese)");
            Console.WriteLine("========================================");
            var japaneseCatalog = new DefaultMessageCatalog("messages_ja.tsv");
            var japaneseValidationCatalog = new ValidationMessageCatalog("Validation_messages_ja.tsv");
            var japaneseApp = new Application(japaneseCatalog, japaneseValidationCatalog);
            japaneseApp.Run();
        }
    }

    internal class Application
    {
        private readonly DefaultMessageCatalog _messageCatalog;
        private readonly ValidationMessageCatalog _validationMessageCatalog;

        public Application(DefaultMessageCatalog messageCatalog, ValidationMessageCatalog validationMessageCatalog)
        {
            _messageCatalog = messageCatalog;
            _validationMessageCatalog = validationMessageCatalog;
        }

        public void Run()
        {
            // Using the message catalog
            Console.WriteLine("=== MessageCatalog ===");
            Console.WriteLine($"{_messageCatalog.INFO001.Text} (Category: {_messageCatalog.INFO001.Category}, Severity: {_messageCatalog.INFO001.Severity})");
            Console.WriteLine($"{_messageCatalog.INFO002.Format("James")} (Category: {_messageCatalog.INFO002.Category}, Severity: {_messageCatalog.INFO002.Severity})");
            Console.WriteLine($"{_messageCatalog.WARN001.Format("file.txt")} (Category: {_messageCatalog.WARN001.Category}, Severity: {_messageCatalog.WARN001.Severity})");
            Console.WriteLine($"{_messageCatalog.WARN002.Format(10)} (Category: {_messageCatalog.WARN002.Category}, Severity: {_messageCatalog.WARN002.Severity})");
            Console.WriteLine($"{_messageCatalog.ERR001.Format("FooBar")} (Category: {_messageCatalog.ERR001.Category}, Severity: {_messageCatalog.ERR001.Severity})");
            Console.WriteLine($"{_messageCatalog.ERR002.Format("HogePiyo.db")} (Category: {_messageCatalog.ERR002.Category}, Severity: {_messageCatalog.ERR002.Severity})");
            Console.WriteLine($"{_messageCatalog.FATAL001.Format("E-005-01")} (Category: {_messageCatalog.FATAL001.Category}, Severity: {_messageCatalog.FATAL001.Severity})");
            
            Console.WriteLine();
            
            // Using the Validation message catalog
            Console.WriteLine("=== ValidationMessageCatalog ===");
            Console.WriteLine($"{_validationMessageCatalog.VALID001.Text} (Category: {_validationMessageCatalog.VALID001.Category}, Severity: {_validationMessageCatalog.VALID001.Severity})");
            Console.WriteLine($"{_validationMessageCatalog.VALID002.Format("username")} (Category: {_validationMessageCatalog.VALID002.Category}, Severity: {_validationMessageCatalog.VALID002.Severity})");
            Console.WriteLine($"{_validationMessageCatalog.VALID003.Format("email")} (Category: {_validationMessageCatalog.VALID003.Category}, Severity: {_validationMessageCatalog.VALID003.Severity})");
        }
    }
}
