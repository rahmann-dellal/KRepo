using KFP.DATA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFP.Services
{
    public interface IPrintingService
    {
        public Task PrintOrderForKitchen(Order order);
        public void PrintOrderForCustomer(Order order);

        public void PrintInvoice(Invoice invoice);

        public List<string> GetAvailablePrinters();
    }
}
