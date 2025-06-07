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
using Microsoft.UI;
using System.Globalization;

namespace KFP.Services
{
    public class PrintingService : IPrintingService
    {
        private AppDataService _appDataService;
        private Currency Currency;

        private string? kitchenPrinterName;
        private string? orderPrinterName;
        private string? ReceiptPrinterName;

        private bool IsKitchenPrinterEnabled;
        private bool IsOrderPrinterEnabled;
        private bool IsReceiptPrinterEnabled;

        private bool PrintEstablishmentNameWithReceipt;
        private bool PrintEstablishmentAddressWithReceipt;
        private bool PrintEstablishmentPhoneNumber1WithReceipt;
        private bool PrintEstablishmentPhoneNumber2WithReceipt;

        private bool PrintEstablishmentNameWithPreBill;
        private bool PrintEstablishmentAddressWithPreBill;
        private bool PrintEstablishmentPhoneNumber1WithPreBill;
        private bool PrintEstablishmentPhoneNumber2WithPreBill;

        private string? EstablishmentName;
        private string? EstablishmentAddress;
        private string? EstablishmentPhoneNumber1;
        private string? EstablishmentPhoneNumber2;

        public PrintingService(AppDataService appDataService)
        {
            _appDataService = appDataService;
            kitchenPrinterName = _appDataService.KitchenPrinterName;
            orderPrinterName = _appDataService.PreBillPrinterName;
            ReceiptPrinterName = _appDataService.ReceiptPrinterName;
            IsKitchenPrinterEnabled = _appDataService.IsKitchenPrinterEnabled;
            IsOrderPrinterEnabled = _appDataService.IsPreBillPrinterEnabled;
            IsReceiptPrinterEnabled = _appDataService.IsReceiptPrinterEnabled;

            PrintEstablishmentNameWithReceipt = _appDataService.PrintEstablishmentNameWithReceipt;
            PrintEstablishmentAddressWithReceipt = _appDataService.PrintEstablishmentAddressWithReceipt;
            PrintEstablishmentPhoneNumber1WithReceipt = _appDataService.PrintEstablishmentPhoneNumber1WithReceipt;
            PrintEstablishmentPhoneNumber2WithReceipt = _appDataService.PrintEstablishmentPhoneNumber2WithReceipt;


            PrintEstablishmentNameWithPreBill = _appDataService.PrintEstablishmentNameWithPreBill;
            PrintEstablishmentAddressWithPreBill = _appDataService.PrintEstablishmentAddressWithPreBill;
            PrintEstablishmentPhoneNumber1WithPreBill = _appDataService.PrintEstablishmentPhoneNumber1WithPreBill;
            PrintEstablishmentPhoneNumber2WithPreBill = _appDataService.PrintEstablishmentPhoneNumber2WithPreBill;

            EstablishmentName = _appDataService.EstablishmentName;
            EstablishmentAddress =_appDataService.EstablishmentAddress;
            EstablishmentPhoneNumber1 = _appDataService.EstablishmentPhoneNumber1;
            EstablishmentPhoneNumber2 = _appDataService.EstablishmentPhoneNumber2;

            Currency = _appDataService.Currency;
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

        public void PrintReceipt(PaymentReceipt invoice)
        {
            throw new NotImplementedException();
        }

        public void PrintPreBill(Order order)
        {
            if (!IsOrderPrinterEnabled || string.IsNullOrEmpty(orderPrinterName))
                return;
            using var printDoc = new PrintDocument();
            printDoc.PrintPage += (s, e) =>
            {
                var g = e.Graphics;
                int pageWidth = (int)e.PageBounds.Width;
                int margin = 10;
                float y = margin;

                var boldFont = new Font("Arial", 10, FontStyle.Bold);
                var regularFont = new Font("Arial", 9);
                var grayPen = new Pen(Color.LightGray) { DashPattern = new float[] { 2, 2 } };

                // Localized labels
                string itemLabel = StringLocalisationService.getStringWithKey("Item");
                string qtyLabel = StringLocalisationService.getStringWithKey("Qty");
                string unitPriceLabel = StringLocalisationService.getStringWithKey("ReceiptUnitPrice");
                string qtyxunitpricelable = qtyLabel + " * " +  unitPriceLabel;
                string notesLabel = StringLocalisationService.getStringWithKey("Notes");
                string totalLabel = StringLocalisationService.getStringWithKey("Total");

                // Establishment info
                if (PrintEstablishmentNameWithPreBill && !string.IsNullOrWhiteSpace(EstablishmentName))
                {
                    DrawWrappedCenteredText(g, EstablishmentName, boldFont, Brushes.Black, pageWidth, ref y);
                }

                if (PrintEstablishmentAddressWithPreBill && !string.IsNullOrWhiteSpace(EstablishmentAddress))
                {
                    DrawWrappedCenteredText(g, EstablishmentAddress, regularFont, Brushes.Black, pageWidth, ref y);
                }

                if (PrintEstablishmentPhoneNumber1WithPreBill && !string.IsNullOrWhiteSpace(EstablishmentPhoneNumber1))
                {
                    DrawWrappedCenteredText(g, EstablishmentPhoneNumber1, regularFont, Brushes.Black, pageWidth, ref y);
                }

                if (PrintEstablishmentPhoneNumber2WithPreBill && !string.IsNullOrWhiteSpace(EstablishmentPhoneNumber2))
                {
                    DrawWrappedCenteredText(g, EstablishmentPhoneNumber2, regularFont, Brushes.Black, pageWidth, ref y);
                }

                y += 10;

                // Centered time
                string timeString = order.SetPreparingAt != null ? ((DateTime)order.SetPreparingAt).ToString(new CultureInfo(_appDataService.AppLanguage)) : string.Empty;
                SizeF timeSize = g.MeasureString(timeString, boldFont);
                float xCentered = (pageWidth - timeSize.Width) / 2;
                g.DrawString(timeString, boldFont, Brushes.Black, xCentered, y);
                y += 20;

                // Centered Order number
                string OrderString = StringLocalisationService.getStringWithKey("Order") + " #" + order.Id;
                SizeF OrderStringSize = g.MeasureString(OrderString, boldFont);
                xCentered = (pageWidth - OrderStringSize.Width) / 2;
                g.DrawString(OrderString, boldFont, Brushes.Black, xCentered, y);
                y += 20;

                // Order location
                string locationLabel = order.orderLocation switch
                {
                    OrderLocation.Table => $"{StringLocalisationService.getStringWithKey("Table")} #{order.TableNumber}",
                    OrderLocation.Counter => StringLocalisationService.getStringWithKey("Counter"),
                    OrderLocation.Delivery => StringLocalisationService.getStringWithKey("Delivery"),
                    _ => string.Empty
                };
                DrawWrappedCenteredText(g, locationLabel, boldFont, Brushes.Black, pageWidth, ref y);

                y += 10;

                if (order.orderLocation == OrderLocation.Delivery && order.DeliveryInfo is DeliveryInfo info)
                {
                    if (!string.IsNullOrWhiteSpace(info.CustomerName)) {
                        string customerNameString = StringLocalisationService.getStringWithKey("CustomerName");
                        DrawWrappedText(g, customerNameString +" : " + info.CustomerName, regularFont, Brushes.Black, margin, pageWidth - 2 * margin, ref y);
                    }
                    if (!string.IsNullOrWhiteSpace(info.PhoneNumber)) {
                        string phoneString = StringLocalisationService.getStringWithKey("Phone");
                        DrawWrappedText(g, phoneString + " : " + info.PhoneNumber, regularFont, Brushes.Black, margin, pageWidth - 2 * margin, ref y);
                    }
                    if (!string.IsNullOrWhiteSpace(info.Address)) {
                        string addressString = StringLocalisationService.getStringWithKey("Address");
                        DrawWrappedText(g, addressString + " : " + info.Address, regularFont, Brushes.Black, margin, pageWidth - 2 * margin, ref y);
                    }
            }
                y += 20;

                // Header
                g.DrawString(itemLabel, boldFont, Brushes.Black, margin, y);
                var qtySize = g.MeasureString(qtyxunitpricelable, boldFont);
                g.DrawString(qtyxunitpricelable, boldFont, Brushes.Black, pageWidth - margin - qtySize.Width, y);
                y += 20;

                g.DrawLine(new Pen(Color.Black, 1) { DashPattern = new float[] { 4, 2 } }, margin, y, pageWidth - margin, y);
                y += 10;

                double total = 0.0;

                foreach (var item in order.OrderItems)
                {
                    DrawWrappedLine(g, item.MenuItemName, item.Quantity, item.UnitPrice, boldFont, pageWidth, margin, ref y);
                    total += item.Quantity * item.UnitPrice;

                    foreach (var addon in item.AddOns)
                    {
                        DrawWrappedLine(g, "+ " + addon.MenuItemName, addon.Quantity, addon.UnitPrice, regularFont, pageWidth, margin + 15, ref y);
                        total += addon.Quantity * addon.UnitPrice;
                    }

                    y += 5;
                    g.DrawLine(grayPen, margin, y, pageWidth - margin, y);
                    y += 5;
                }

                y += 10;

                // Total
                var totalText = $"{totalLabel}: {order.TotalPrice?.ToString("F2") ?? total.ToString("F2")} {Currency}";
                var totalSize = g.MeasureString(totalText, boldFont);
                g.DrawString(totalText, boldFont, Brushes.Black, pageWidth - margin - totalSize.Width, y);
                y += 20;

                // Notes
                if (!string.IsNullOrWhiteSpace(order.Notes))
                {
                    g.DrawString(notesLabel + ":", boldFont, Brushes.Black, margin, y);
                    y += 18;
                    DrawWrappedText(g, order.Notes, regularFont, Brushes.Black, margin, pageWidth - 2 * margin, ref y);
                }
            };

            printDoc.Print();
        }

        // Helper methods
        private void DrawWrappedCenteredText(Graphics g, string text, Font font, Brush brush, int maxWidth, ref float y)
        {
            SizeF textSize = g.MeasureString(text, font, maxWidth - 20);
            RectangleF layoutRect = new RectangleF(0, y, maxWidth, textSize.Height);
            var sf = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString(text, font, brush, layoutRect, sf);
            y += textSize.Height;
        }

        private void DrawWrappedText(Graphics g, string text, Font font, Brush brush, float x, float width, ref float y)
        {
            SizeF textSize = g.MeasureString(text, font, (int)width);
            RectangleF layoutRect = new RectangleF(x, y, width, textSize.Height);
            g.DrawString(text, font, brush, layoutRect);
            y += textSize.Height;
        }

        private void DrawWrappedLine(Graphics g, string name, int qty, double price, Font font, int pageWidth, int x, ref float y)
        {
            string qtyText = $"{qty} x {price:F2}";
            SizeF qtySize = g.MeasureString(qtyText, font);
            float rightX = pageWidth - x - qtySize.Width;

            // Wrap name
            int availableWidth = (int)(rightX - x - 5);
            SizeF nameSize = g.MeasureString(name, font, availableWidth);
            RectangleF nameRect = new RectangleF(x, y, availableWidth, nameSize.Height);

            g.DrawString(name, font, Brushes.Black, nameRect);
            g.DrawString(qtyText, font, Brushes.Black, rightX, y);
            y += nameSize.Height;
        }


        public void PrintKitchenTicket(Order order)
        {
            if (!IsKitchenPrinterEnabled || string.IsNullOrEmpty(kitchenPrinterName))
                return;

            PrintDocument printDoc = new PrintDocument();
            printDoc.PrinterSettings.PrinterName = kitchenPrinterName;

            string qtyLabel = StringLocalisationService.getStringWithKey("Qty");
            string ItemLabel = StringLocalisationService.getStringWithKey("Item");
            string NotesLabel = StringLocalisationService.getStringWithKey("Notes");

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

                // Centered Order number
                string OrderString = StringLocalisationService.getStringWithKey("Order") + " #" + order.Id; 
                SizeF OrderStringSize = g.MeasureString(OrderString, boldFont);
                xCentered = (pageWidth - OrderStringSize.Width) / 2;
                g.DrawString(OrderString, boldFont, Brushes.Black, xCentered, y);
                y += lineHeight * 1.5f;

                // Header: "Item" and "Qty"
                g.DrawString(ItemLabel, boldFont, Brushes.Black, margin, y);
                float qtyWidth = g.MeasureString(qtyLabel, boldFont).Width;
                g.DrawString(qtyLabel, boldFont, Brushes.Black, pageWidth - margin - qtyWidth, y);
                y += lineHeight;

                // Black dashed line under header
                g.DrawLine(blackDashPen, margin, y, pageWidth - margin, y);
                y += lineHeight / 2;

                foreach (var item in order.OrderItems)
                {
                    // Print item name (wrapped) and quantity
                    y = DrawWrappedLine2(g, item.MenuItemName, item.Quantity, font, margin, y, usableWidth, pageWidth);

                    // Add-ons
                    foreach (var addon in item.AddOns)
                    {
                        string addonText = "+ " + addon.MenuItemName;
                        y = DrawWrappedLine2(g, addonText, addon.Quantity, font, margin + 20, y, usableWidth - 20, pageWidth);
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
                    g.DrawString(NotesLabel + ":", boldFont, Brushes.Black, margin, y);
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

        private float DrawWrappedLine2(Graphics g, string text, int quantity, Font font, float x, float y, float maxWidth, float pageWidth)
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
