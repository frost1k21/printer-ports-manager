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
            return GetPrinterInfoFromQuery(wsName, "Select Name, ShareName, PortName from win32_printer");
        }

        public IEnumerable<PrinterInfo> GetPrinterInfoWithShareName(string wsName)
        {
            return GetPrinterInfoFromQuery(wsName, "Select Name, ShareName, PortName from win32_printer where ShareName != null");
        }

        public IEnumerable<PrinterInfo> GetPrinterInfoWithTcpIpPort(string wsName)
        {
            var printerInfoCollection = new List<PrinterInfo>();
            var tcpIpPrinterPorts = GetTcpIpPrinterPorts(wsName);

            if (tcpIpPrinterPorts.Count() == 0)
                return printerInfoCollection;
            foreach (var item in tcpIpPrinterPorts)
            {
                var tcpIpPrinters = GetPrinterInfoFromQuery(wsName, $@"Select Name, ShareName, PortName from win32_printer where PortName = ""{item.portName}""");
                if (tcpIpPrinters.Count() != 0)
                    printerInfoCollection.AddRange(tcpIpPrinters);
            }

            return printerInfoCollection;
        }

        private IEnumerable<PrinterInfo> GetPrinterInfoFromQuery(string wsName, string query)
        {
            var listOfPrinterOnfo = new List<PrinterInfo>();

            var scope = new ManagementScope($@"\\{wsName}\root\cimv2");
            scope.Connect();

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

        private IEnumerable<(string portName, string hostAddress)> GetTcpIpPrinterPorts(string wsName)
        {
            var tcpIpPrinterPortCollection = new List<(string portName, string hostAddress)>();
            var scope = new ManagementScope($@"\\{wsName}\root\cimv2");
            scope.Connect();

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
