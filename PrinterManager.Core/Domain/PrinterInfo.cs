using System;

namespace PrinterManager.Core.Domain
{
    public class PrinterInfo
    {
        public string Name { get; }
        public string ShareName { get; }
        public string PortName { get; }

        public PrinterInfo(string name, string shareName, string portName)
        {
            Name = name;
            ShareName = shareName;
            PortName = portName;
        }
              

        public override string ToString()
        {
            return $"Имя принтера: {Name}{Environment.NewLine}Имя общего ресурса: {ShareName}{Environment.NewLine}Имя порта: {PortName}";
        }
    }
}
