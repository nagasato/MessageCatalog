using MessageCatalog.Core;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // メッセージカタログをインスタンス化
            var messageCatalog = new DefaultMessageCatalog();
            var validationMessageCatalog = new ValidationMessageCatalog();

            // アプリケーションを実行
            var app = new Application(messageCatalog, validationMessageCatalog);
            app.Run();
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
            // 既存のメッセージ管理クラスの使用方法
            Console.WriteLine("=== MessageCatalog ===");
            Console.WriteLine($"{_messageCatalog.INFO001.Text} (Category: {_messageCatalog.INFO001.Category}, Severity: {_messageCatalog.INFO001.Servity}, Description: {_messageCatalog.INFO001.Description})");
            Console.WriteLine($"{_messageCatalog.INFO002.Format("James")} (Category: {_messageCatalog.INFO002.Category}, Severity: {_messageCatalog.INFO002.Servity}, Description: {_messageCatalog.INFO002.Description})");
            Console.WriteLine($"{_messageCatalog.WARN001.Format("file.txt")} (Category: {_messageCatalog.WARN001.Category}, Severity: {_messageCatalog.WARN001.Servity}, Description: {_messageCatalog.WARN001.Description})");
            Console.WriteLine($"{_messageCatalog.WARN002.Format(10)} (Category: {_messageCatalog.WARN002.Category}, Severity: {_messageCatalog.WARN002.Servity}, Description: {_messageCatalog.WARN002.Servity})");
            Console.WriteLine($"{_messageCatalog.ERR001.Format("FooBar")} (Category: {_messageCatalog.ERR001.Category}, Severity: {_messageCatalog.ERR001.Servity}, Description: {_messageCatalog.ERR001.Description})");
            Console.WriteLine($"{_messageCatalog.ERR002.Format("HogePiyo.db")} (Category: {_messageCatalog.ERR002.Category}, Severity: {_messageCatalog.ERR002.Servity}, Description: {_messageCatalog.ERR002.Description})");
            Console.WriteLine($"{_messageCatalog.FATAL001.Format("E-005-01")} (Category: {_messageCatalog.FATAL001.Category}, Severity: {_messageCatalog.FATAL001.Servity}, Description: {_messageCatalog.FATAL001.Description})");
            
            Console.WriteLine();
            
            // 新しいValidationメッセージカタログの使用
            Console.WriteLine("=== ValidationMessageCatalog ===");
            Console.WriteLine($"{_validationMessageCatalog.VALID001.Text} (Category: {_validationMessageCatalog.VALID001.Category}, Severity: {_validationMessageCatalog.VALID001.Servity}, Description: {_validationMessageCatalog.VALID001.Description})");
            Console.WriteLine($"{_validationMessageCatalog.VALID002.Format("ユーザー名")} (Category: {_validationMessageCatalog.VALID002.Category}, Severity: {_validationMessageCatalog.VALID002.Servity}, Description: {_validationMessageCatalog.VALID002.Description})");
            Console.WriteLine($"{_validationMessageCatalog.VALID003.Format("メールアドレス")} (Category: {_validationMessageCatalog.VALID003.Category}, Severity: {_validationMessageCatalog.VALID003.Servity}, Description: {_validationMessageCatalog.VALID003.Description})");
        }
    }
}
