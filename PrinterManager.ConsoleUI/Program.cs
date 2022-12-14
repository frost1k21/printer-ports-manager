using PrinterManager.Core;
using System.Linq;
using System;
using System.Runtime.Versioning;

namespace PrinterManager.ConsoleUI
{
    class Program
    {
        [SupportedOSPlatform("windows")]
        static void Main(string[] args)
        {
            Console.WriteLine("Вас приветсвует программа для управлению принтерами");
            CreatePrinterPort();
            //GetPrinterInfo();
            Console.WriteLine("Для выхода нажмите любую клавишу...");
            Console.ReadKey(false);
        }

        private static void CreatePrinterPort()
        {
            Console.Write("Введите имя порта для принтера: ");
            var printerPortName = Console.ReadLine();
            Console.Write("Введите имя компьютера для добавления порта принтера: ");
            var targetComputerName = Console.ReadLine();


            var printerPortManager = new PrinterPortManager();

            var createPrinterPortResult = printerPortManager.CreatePortNew(printerPortName, targetComputerName, false);


            Console.WriteLine(createPrinterPortResult);
        }
        [SupportedOSPlatform("windows")]
        private static void GetPrinterInfo()
        {
            Console.WriteLine("Сбор информации по принетрам");
            Console.Write("Введите имя целевого компьютера: ");
            var wsName = Console.ReadLine();
            var printerInfoCollector = new PrinterInfoCollector();
            var result = printerInfoCollector.GetPrintersInfoWithTcpIpPort(wsName);
            Console.WriteLine();
            Console.WriteLine("Результаты запроса:");
            Console.WriteLine();
            if (result.Count() != 0)
            {
                foreach (var item in result)
                {
                    Console.WriteLine(item);
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Пустая коллекция");
                Console.WriteLine();
            }
        }
    }
}
