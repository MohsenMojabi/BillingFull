using Billing.Models.Authentication;
using Billing.Models.Models;
using Billing.Utility;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {

            base.OnModelCreating(builder);

            //Fluent api

            builder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);

                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);

                entity.HasQueryFilter(c => c.Status == StatusEnum.Approved);
            });

            builder.Entity<Invoice>(entity =>
            {
                entity.Property(e => e.Amount).IsRequired();

                entity.Property(e => e.DeadLine)
                //.HasColumnType("Date")
                .HasConversion(dt => dt, dt => dt.AddTicks(-dt.Ticks % TimeSpan.TicksPerSecond));

                entity.HasQueryFilter(c => c.Status == StatusEnum.Approved);

            });

        }
    }
}
