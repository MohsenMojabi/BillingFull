using Billing.DataAccess.Authentication;
using Billing.DataAccess.Data;
using Billing.DataAccess.Repository.IRepository;
using Billing.Models.Authentication;
using Billing.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billing.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly IAuthenticationServices _authenticationServices;

        public DbInitializer(ApplicationDbContext db, IAuthenticationServices authenticationServices)
        {
            _db = db;
            _authenticationServices = authenticationServices;
        }
        public void InitializeDb()
        {
            //Migrations if they are not applied
            try
            {
                _db.Database.EnsureDeleted();
                _db.Database.Migrate();
                //if (_db.Database.GetPendingMigrations().Count() > 0)
                //{
                //    _db.Database.Migrate();
                //}
            }
            catch (Exception)
            {
                //var gg = ex.Message;
            }


            //Seed the database
            for (int i = 1; i <= 100; i++)
            {
                var customer = new Customer()
                {
                    FirstName = String.Format("Name{0}", i),
                    LastName = String.Format("Family{0}", i),
                    Email = String.Format("Family{0}@example.com", i),
                    ImgUrl = ""
                };
                List<Invoice> invoiceList = new List<Invoice>();
                int rand = new Random().Next(1, 10);
                for (int j = 1; j <= rand; j++)
                {
                    invoiceList.Add(new Invoice()
                    {
                        Amount = j * 1000,
                        DeadLine = DateTime.Now.AddDays(j * 10)//,
                        //Customer = customer
                    }); ;
                }
                customer.InvoiceList = invoiceList;
                _db.Customers.Add(customer);
            }
            _db.SaveChanges();


            // Creating admin role and admin user
            _ = _authenticationServices.RegisterAsync(new Register()
            {
                Username = "mojabi",
                Email = "mojabi@example.com",
                Password = "123!@#qweQWE"
            }, UserRoles.Admin).Result;

            // Creating user role and normal user
            _ = _authenticationServices.RegisterAsync(new Register()
            {
                Username = "user",
                Email = "user@example.com",
                Password = "123!@#qweQWE"
            }, UserRoles.User).Result;
        }
    }
}
