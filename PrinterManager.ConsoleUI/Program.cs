using PrinterManager.Core;
using System;

namespace PrinterManager.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Вас приветсвует программа для управлению принтерами");
            Console.Write("Введите имя порта для принтера: ");
            var printerPortName = Console.ReadLine();
            Console.Write("Введите имя компьютера для добавления порта принтера: ");
            var targetComputerName = Console.ReadLine();


            var printerPortManager = new PrinterPortManager();

            var createPrinterPortResult = printerPortManager.CreatePort(printerPortName, targetComputerName, false);


            Console.WriteLine(createPrinterPortResult);

            Console.WriteLine("Для выхода нажмите любую клавишу...");
            Console.ReadKey(false);
        }
    }
}
