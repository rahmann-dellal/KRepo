using KFP.DATA;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using System.IO;
using WinRT;
using System.Runtime.InteropServices.WindowsRuntime;
using KFP.Printables;
using Microsoft.UI;
using System.Globalization;

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
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                printers.Add(printer);
            }
            return printers;
        }

        public void PrintInvoice(Invoice invoice)
        {
            throw new NotImplementedException();
        }

        public void PrintOrderForCustomer(Order order)
        {
            if (!IsOrderPrinterEnabled || string.IsNullOrEmpty(orderPrinterName))
                return;
        }

        public async Task PrintOrderForKitchen(Order order)
        {
            if (!IsKitchenPrinterEnabled || string.IsNullOrEmpty(kitchenPrinterName))
                return;

            PrintDocument printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = kitchenPrinterName;

            printDoc.PrintPage += (sender, e) =>
            {
                Graphics g = e.Graphics;
                Font font = new Font("Consolas", 10);
                Font boldFont = new Font("Consolas", 10, FontStyle.Bold);
                Pen grayDashPen = new Pen(Color.Gray, 1) { DashPattern = new float[] { 2, 2 } };
                Pen blackDashPen = new Pen(Color.Black, 1) { DashPattern = new float[] { 2, 2 } };

                float margin = 20;
                float y = margin;
                float lineHeight = font.GetHeight(g) + 6;
                float pageWidth = e.PageBounds.Width;
                float usableWidth = pageWidth - (2 * margin);

                // Centered time
                string timeString = order.SetPreparingAt != null ? ((DateTime)order.SetPreparingAt).ToString(new CultureInfo(_appDataService.AppLanguage)) : string.Empty;
                SizeF timeSize = g.MeasureString(timeString, boldFont);
                float xCentered = (pageWidth - timeSize.Width) / 2;
                g.DrawString(timeString, boldFont, Brushes.Black, xCentered, y);
                y += lineHeight * 1.5f;

                // Header: "Item" and "Qty"
                g.DrawString("Item", boldFont, Brushes.Black, margin, y);
                string qtyLabel = "Qty";
                float qtyWidth = g.MeasureString(qtyLabel, boldFont).Width;
                g.DrawString(qtyLabel, boldFont, Brushes.Black, pageWidth - margin - qtyWidth, y);
                y += lineHeight;

                // Black dashed line under header
                g.DrawLine(blackDashPen, margin, y, pageWidth - margin, y);
                y += lineHeight / 2;

                foreach (var item in order.OrderItems)
                {
                    // Print item name (wrapped) and quantity
                    y = DrawWrappedLine(g, item.MenuItemName, item.Quantity, font, margin, y, usableWidth, pageWidth);

                    // Add-ons
                    foreach (var addon in item.AddOns)
                    {
                        string addonText = "+ " + addon.MenuItemName;
                        y = DrawWrappedLine(g, addonText, addon.Quantity, font, margin + 20, y, usableWidth - 20, pageWidth);
                    }

                    // Gray dashed separator
                    y += 4;
                    g.DrawLine(grayDashPen, margin, y, pageWidth - margin, y);
                    y += lineHeight / 2;
                }

                // Notes section
                if (!string.IsNullOrWhiteSpace(order.Notes))
                {
                    y += lineHeight;
                    g.DrawString("Notes:", boldFont, Brushes.Black, margin, y);
                    y += lineHeight;

                    y = DrawWrappedTextBlock(g, order.Notes, font, margin + 10, y, usableWidth - 10);
                }

                y += margin;
            };

            try
            {
                printDoc.Print();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Printing failed: {ex.Message}");
            }
        }

        private float DrawWrappedLine(Graphics g, string text, int quantity, Font font, float x, float y, float maxWidth, float pageWidth)
        {
            string qtyStr = quantity.ToString();
            float qtyWidth = g.MeasureString(qtyStr, font).Width;

            List<string> lines = WrapText(g, text, font, maxWidth - qtyWidth - 10);
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                g.DrawString(line, font, Brushes.Black, x, y);

                if (i == 0)
                    g.DrawString(qtyStr, font, Brushes.Black, pageWidth - x - qtyWidth, y);

                y += font.GetHeight(g) + 4;
            }

            return y;
        }

        private float DrawWrappedTextBlock(Graphics g, string text, Font font, float x, float y, float maxWidth)
        {
            List<string> lines = WrapText(g, text, font, maxWidth);
            foreach (var line in lines)
            {
                g.DrawString(line, font, Brushes.Black, x, y);
                y += font.GetHeight(g) + 4;
            }

            return y;
        }

        private List<string> WrapText(Graphics g, string text, Font font, float maxWidth)
        {
            var words = text.Split(' ');
            var lines = new List<string>();
            string currentLine = "";

            foreach (var word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                float width = g.MeasureString(testLine, font).Width;

                if (width > maxWidth)
                {
                    if (!string.IsNullOrEmpty(currentLine))
                        lines.Add(currentLine);

                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
                lines.Add(currentLine);

            return lines;
        }

    }
}
