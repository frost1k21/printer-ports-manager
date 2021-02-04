using System.Management.Automation;

namespace PrinterManager.Core
{
    public interface IPrinterPortManager
    {
        string CreatePort(string printerPortName, string targetComputerName, bool isNetwork);
    }

    public class PrinterPortManager : IPrinterPortManager
    {
        public string CreatePort(string printerPortName, string targetComputerName, bool isNetwork)
        {
            var ps = PowerShell.Create();
            ps.AddScript("Set-ExecutionPolicy Unrestricted")
                .AddStatement().AddScript("Import-Module PrintManagement");
            if (isNetwork)
            {
                ps.AddStatement().AddScript($"Add-PrinterPort -Name \"{printerPortName}\" -ComputerName {targetComputerName} -PrinterHostAddress \"{printerPortName}\"");
            }
            else
            {
                ps.AddStatement().AddScript($"Add-PrinterPort -Name \"{printerPortName}\" -ComputerName {targetComputerName}");
            }
            ps.AddStatement().AddScript("Set-ExecutionPolicy Restricted");

            
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

        public string CreatePortNew(string printerPortName, string targetComputerName, bool isNetwork)
        {
            var ps = PowerShell.Create();
            ps.AddScript("Set-ExecutionPolicy Unrestricted")
                .AddStatement().AddScript("Import-Module PrintManagement");
            
            if (isNetwork)
            {
                ps.AddStatement().AddScript($"Add-PrinterPort -Name \"{printerPortName}\" -ComputerName {targetComputerName} -PrinterHostAddress \"{printerPortName}\"");
            }
            else
            {
                var cimPath = $"[wmiclass]\"\\\\{targetComputerName}\\root\\default:stdregprov\"";
                var script = $"({cimPath}).SetStringValue(2147483650, \"SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Ports\", \"{printerPortName}\", \"\")";
                var stopServiceScript = $"([wmi]'\\\\{targetComputerName}\\root\\cimv2:win32_service.Name=\"spooler\"').StopService()";
                var startServiceScript = $"([wmi]'\\\\{targetComputerName}\\root\\cimv2:win32_service.Name=\"spooler\"').StartService()";
                ps.AddStatement().AddScript(script)
                        .AddStatement().AddScript(stopServiceScript)
                        .AddStatement().AddScript(startServiceScript);
            }
            ps.AddStatement().AddScript("Set-ExecutionPolicy Restricted");

            var result = "";

            ps.Invoke();
            var errors = ps.Streams.Error.ReadAll();
            if (errors != null && errors.Count > 0)
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
