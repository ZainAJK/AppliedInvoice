using AppliedInvoice.Models;
using Microsoft.EntityFrameworkCore;

namespace AppliedInvoice.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<InvoiceMaster> Invoices { get; set; }
        public DbSet<InvoiceDetails> InvoiceDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceDetails>()
                .HasOne<InvoiceMaster>()
                .WithMany(x => x.items)
                .HasForeignKey(x => x.InvoiceMasterId);
        }
    }
}