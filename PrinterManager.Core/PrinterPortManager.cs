using System.Management.Automation;

namespace PrinterManager.Core
{
    public interface IPrinterPortManager
    {
        string CreatePort(string printerPortName, string targetComputerName);
    }

    public class PrinterPortManager : IPrinterPortManager
    {
        public string CreatePort(string printerPortName, string targetComputerName)
        {
            var ps = PowerShell.Create();
            ps.AddScript("Set-ExecutionPolicy Unrestricted")
                .AddStatement().AddScript("Import-Module PrintManagement")
                .AddStatement().AddScript($"Add-PrinterPort -Name {printerPortName} -ComputerName {targetComputerName}")
                .AddStatement().AddScript("Set-ExecutionPolicy Restricted");

            
            var result = "";

            ps.Invoke();
            var errors = ps.Streams.Error.ReadAll();
            if(errors != null && errors.Count > 0)
            {
                foreach (var error in errors)
                {
                    result += error.Exception.Message;
                }
            }
            else
            {
                result = $"Добаление порта - {printerPortName} на {targetComputerName} прошло успешно";
            }

            return result;
        }
    }
}
