using Invoices.Models;
using Microsoft.EntityFrameworkCore;

namespace Invoices.DataAccess
{
    public class InvoicesContext : DbContext
    {
        public InvoicesContext(DbContextOptions<InvoicesContext> options) : base(options)
        {
        }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceEntry> InvoiceEntries { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Product> Products{ get; set; }
    }
}