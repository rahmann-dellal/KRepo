using KFP.DATA;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace KFP.DATA_Access
{
    public class KFPContext : DbContext
    {
        public string DbPath;
        private string fileName = "KFP.db";

        public KFPContext()
        {
            var folder = Environment.SpecialFolder.ApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, fileName);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<DeliveryInfo> DeliveryInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Order ↔ OrderItem (Cascade delete)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Invoice → Order (Set null on delete)
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Order)
                .WithMany() // Invoice does not need a navigation property in Order
                .OnDelete(DeleteBehavior.SetNull);

            // OrderItem → MenuItem
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany()
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.SetNull);

            // Sale → MenuItem
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.MenuItem)
                .WithMany()
                .HasForeignKey(s => s.MenuItemId)
                .OnDelete(DeleteBehavior.SetNull);

            // Sale → Invoice
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Invoice)
                .WithMany(i => i.Sales)
                .HasForeignKey(s => s.InvoiceId)
                .OnDelete(DeleteBehavior.SetNull); 

            // Invoice → AppUser
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.AppUser)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.AppUserId)
                .OnDelete(DeleteBehavior.SetNull); // Keep invoice even if user is deleted

            // Order → AppUser
            modelBuilder.Entity<Order>()
                .HasOne(o => o.AppUser)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.AppUserId)
                .OnDelete(DeleteBehavior.SetNull); // Keep Order even if user is removed

            // One-to-one: Order ↔ DeliveryInfo
            modelBuilder.Entity<Order>()
                .HasOne(o => o.DeliveryInfo)
                .WithOne(d => d.Order)
                .HasForeignKey<DeliveryInfo>(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.ParentOrderItem)
                .WithMany(oi => oi.ChildrenOrderItems)
                .HasForeignKey(oi => oi.ParentOrderItemId)
                .OnDelete(DeleteBehavior.Cascade); 

        }
    }
}
