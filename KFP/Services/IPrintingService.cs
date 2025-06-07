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
        public void PrintKitchenTicket(Order order);
        public void PrintPreBill(Order order);

        public void PrintReceipt(PaymentReceipt receipt);

        public List<string> GetAvailablePrinters();
    }
}
