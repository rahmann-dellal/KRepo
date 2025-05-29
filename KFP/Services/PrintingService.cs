using KFP.DATA;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.Services
{
    public class PrintingService : IPrintingService
    {
        private AppDataService _appDataService;
        
        private string? kitchenPrinterName;
        private string? orderPrinterName;
        private string? invoicePrinterName;

        private bool IsKitchenPrinterEnabled;
        private bool IsOrderPrinterEnabled;
        private bool IsInvoicePrinterEnabled;

        public PrintingService(AppDataService appDataService)
        {
            _appDataService = appDataService;
            kitchenPrinterName = _appDataService.KitchenPrinterName;
            orderPrinterName = _appDataService.OrderPrinterName;
            invoicePrinterName = _appDataService.InvoicePrinterName;
            IsKitchenPrinterEnabled = _appDataService.IsKitchenPrinterEnabled;
            IsOrderPrinterEnabled = _appDataService.IsOrderPrinterEnabled;
            IsInvoicePrinterEnabled = _appDataService.IsInvoicePrinterEnabled;
        }

        public List<string> GetAvailablePrinters()
        {
            var printers = new List<string>();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                printers.Add(printer);
            }
            return printers;
        }

        public void PrintInvoice(Invoice invoice)
        {
            throw new NotImplementedException();
        }

        public void PrintOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
