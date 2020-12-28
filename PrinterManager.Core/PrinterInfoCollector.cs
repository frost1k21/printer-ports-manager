using PrinterManager.Core.Domain;
using System.Collections.Generic;
using System.Management;
using System.Runtime.Versioning;
using System.Linq;

namespace PrinterManager.Core
{
    [SupportedOSPlatform("windows")]
    public class PrinterInfoCollector
    {
        public IEnumerable<PrinterInfo> GetPrinterInfo(string wsName)
        {
            var scope = GetMangementScope(wsName);
            return GetPrinterInfoFromQuery(wsName, "Select Name, ShareName, PortName from win32_printer", scope);
        }

        public IEnumerable<PrinterInfo> GetPrinterInfoWithShareName(string wsName)
        {
            var scope = GetMangementScope(wsName);
            return GetPrinterInfoFromQuery(wsName, "Select Name, ShareName, PortName from win32_printer where ShareName != null", scope);
        }

        public IEnumerable<PrinterInfo> GetPrinterInfoWithTcpIpPort(string wsName)
        {
            var scope = GetMangementScope(wsName);
            var printerInfoCollection = new List<PrinterInfo>();
            var tcpIpPrinterPorts = GetTcpIpPrinterPorts(wsName, scope);

            if (tcpIpPrinterPorts.Count() == 0)
                return printerInfoCollection;
            foreach (var item in tcpIpPrinterPorts)
            {
                var tcpIpPrinters = GetPrinterInfoFromQuery(wsName, $@"Select Name, ShareName, PortName from win32_printer where PortName = ""{item.portName}""", scope);
                if (tcpIpPrinters.Count() != 0)
                    printerInfoCollection.AddRange(tcpIpPrinters);
            }

            return printerInfoCollection;
        }

        private ManagementScope GetMangementScope(string wsName)
        {
            var scope = new ManagementScope($@"\\{wsName}\root\cimv2");
            scope.Connect();
            return scope;
        }
        private IEnumerable<PrinterInfo> GetPrinterInfoFromQuery(string wsName, string query, ManagementScope scope)
        {
            var listOfPrinterOnfo = new List<PrinterInfo>();

            var selectQuery = new ObjectQuery(query);

            var searchResult = new ManagementObjectSearcher(scope, selectQuery).Get();
            foreach (var item in searchResult)
            {
                if (item["Name"] == null || item["PortName"] == null) 
                    continue;
                var name = item["Name"].ToString();
                var shareName = (item["ShareName"] == null) ? "" : item["ShareName"].ToString();
                var portName = item["PortName"].ToString();
                var printerInfo = new PrinterInfo(
                   name,
                   shareName,
                   portName
                );
                listOfPrinterOnfo.Add(printerInfo);
            }

            return listOfPrinterOnfo;
        }

        private IEnumerable<(string portName, string hostAddress)> GetTcpIpPrinterPorts(string wsName, ManagementScope scope)
        {
            var tcpIpPrinterPortCollection = new List<(string portName, string hostAddress)>();

            var selectQuery = new ObjectQuery("Select Name, HostAddress from win32_tcpipprinterport");
            var searchResult = new ManagementObjectSearcher(scope, selectQuery).Get();
            if (searchResult.Count == 0) 
                return tcpIpPrinterPortCollection;
            foreach (var item in searchResult)
            {
                var portName = item["Name"] == null ? "" : item["Name"].ToString();
                var hostAddress = item["HostAddress"] == null ? "" : item["HostAddress"].ToString();
                tcpIpPrinterPortCollection.Add((portName, hostAddress));
            }
            return tcpIpPrinterPortCollection;
        }
    }
}
