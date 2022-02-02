using Billing.DataAccess.Data;
using Billing.DataAccess.Repository;
using Billing.DataAccess.Repository.IRepository;
using Billing.Models.Models;
using Billing.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Billing.DataAccess.Test
{
    public class UnitOfWork_Invoice_Test
    {
        private DbContextOptions<ApplicationDbContext> options;
        private ApplicationDbContext dbContext;
        private IUnitOfWork unitOfWork;

        //public UnitOfWork_Test()
        //{
        //    options = new DbContextOptionsBuilder<ApplicationDbContext>()
        //    .UseInMemoryDatabase(databaseName: "Get_ShouldReturnAllCustomers_WithoutSearchedTermParameter").Options;
        //    dbContext = new ApplicationDbContext(options);
        //    unitOfWork = new UnitOfWork(dbContext);
        //}

        [Fact]
        public async Task AddAsync_ShouldAddNewInvoice()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "AddAsync_ShouldAddNewInvoice").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            Invoice invoice = null;
            #endregion


            #region Act
            Guid guid = Guid.NewGuid();
            await unitOfWork.Invoice.AddAsync(new Invoice()
            {
                Id = guid,
                Amount = 1000,
                DeadLine = DateTime.Now
            });
            await unitOfWork.SaveAsync();
            invoice = await unitOfWork.Invoice.GetFirstOrDefaultAsync(p => p.Id.Equals(guid));
            #endregion

            #region Assert
            Assert.NotNull(invoice);
            Assert.Equal(1000, invoice.Amount);
            #endregion

        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllInvoices()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "GetAllAsync_ShouldReturnAllInvoices").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.Invoice.AddAsync(new Invoice()
            {
                Id = Guid.NewGuid(),
                Amount = 1000
            });
            await unitOfWork.Invoice.AddAsync(new Invoice()
            {
                Id = Guid.NewGuid(),
                Amount = 2000
            });
            await unitOfWork.Invoice.AddAsync(new Invoice()
            {
                Id = Guid.NewGuid(),
                Amount = 3000
            });
            await unitOfWork.SaveAsync();
            IEnumerable<Invoice> all_Invoices = null;
            #endregion


            #region Act
            all_Invoices = await unitOfWork.Invoice.GetAllAsync();
            #endregion

            #region Assert
            Assert.Equal(3, all_Invoices.Count());
            #endregion
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllInvoices_WithSpecificFilter()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "GetAllAsync_ShouldReturnAllInvoices_WithSpecificFilter").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.Invoice.AddAsync(new Invoice()
            {
                Id = Guid.NewGuid(),
                Amount = 1000
            });
            await unitOfWork.Invoice.AddAsync(new Invoice()
            {
                Id = Guid.NewGuid(),
                Amount = 1000
            });
            await unitOfWork.Invoice.AddAsync(new Invoice()
            {
                Id = Guid.NewGuid(),
                Amount = 2000
            });
            await unitOfWork.SaveAsync();
            IEnumerable<Invoice> all_invoices_withSpesificFilter = null;
            #endregion


            #region Act
            all_invoices_withSpesificFilter = await unitOfWork.Invoice.GetAllAsync(p => p.Amount.Equals(1000));
            #endregion

            #region Assert
            Assert.Equal(2, all_invoices_withSpesificFilter.Count());
            Assert.Equal(1000, all_invoices_withSpesificFilter.ElementAt(1).Amount);
            #endregion
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllInvoices_WithIncludedProperty()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "GetAllAsync_ShouldReturnAllInvoices_WithIncludedProperty").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.Customer.AddAsync(new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = "name1",
                LastName = "family1",
                InvoiceList = new List<Invoice>()
                {
                    new Invoice(){
                        Amount = 1000,
                        DeadLine = DateTime.Now
                    },
                    new Invoice(){
                        Amount = 2000,
                        DeadLine = DateTime.Now
                    },
                    new Invoice(){
                        Amount = 3000,
                        DeadLine = DateTime.Now
                    }
                }
            });
            await unitOfWork.Customer.AddAsync(new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = "name2",
                LastName = "family2",
                InvoiceList = new List<Invoice>()
                {
                    new Invoice(){
                        Amount = 4000,
                        DeadLine = DateTime.Now
                    },
                    new Invoice(){
                        Amount = 5000,
                        DeadLine = DateTime.Now
                    },
                    new Invoice(){
                        Amount = 6000,
                        DeadLine = DateTime.Now
                    }
                }
            });
            await unitOfWork.SaveAsync();
            IEnumerable<Invoice> all_invoices_withIncludeProperties = null;
            #endregion


            #region Act
            all_invoices_withIncludeProperties = await unitOfWork.Invoice.GetAllAsync(includeProperties: "Customer");
            #endregion

            #region Assert
            Assert.NotNull(all_invoices_withIncludeProperties.FirstOrDefault().Customer);
            Assert.Equal("name1", all_invoices_withIncludeProperties.FirstOrDefault().Customer.FirstName);
            Assert.Equal("family2", all_invoices_withIncludeProperties.ElementAt(4).Customer.LastName);
            #endregion
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_ShouldReturnOneInvoice_WithSpecificFilter()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "GetFirstOrDefaultAsync_ShouldReturnOneInvoice_WithSpecificFilter").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            Guid guid = Guid.NewGuid();
            await unitOfWork.Invoice.AddAsync(new Invoice()
            {
                Id = Guid.NewGuid(),
                Amount = 1000
            });
            await unitOfWork.Invoice.AddAsync(new Invoice()
            {
                Id = guid,
                Amount = 2000
            });
            await unitOfWork.Invoice.AddAsync(new Invoice()
            {
                Id = Guid.NewGuid(),
                Amount = 3000
            });
            await unitOfWork.SaveAsync();
            Invoice invoice_withSpesificFilter = null;
            #endregion


            #region Act
            invoice_withSpesificFilter = await unitOfWork.Invoice.GetFirstOrDefaultAsync(p => p.Id.Equals(guid));
            #endregion

            #region Assert
            Assert.NotNull(invoice_withSpesificFilter);
            Assert.Equal(2000, invoice_withSpesificFilter.Amount);
            #endregion
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_ShouldReturnOneInvoice_WithIncludedProperty()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "GetFirstOrDefaultAsync_ShouldReturnOneInvoice_WithIncludedProperty").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.Customer.AddAsync(new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = "name1",
                LastName = "family1",
                InvoiceList = new List<Invoice>()
                {
                    new Invoice(){
                        Amount = 1000,
                        DeadLine = DateTime.Now
                    },
                    new Invoice(){
                        Amount = 2000,
                        DeadLine = DateTime.Now
                    },
                    new Invoice(){
                        Amount = 3000,
                        DeadLine = DateTime.Now
                    }
                }
            });
            await unitOfWork.SaveAsync();
            Invoice invoice_withIncludeProperties = null;
            #endregion


            #region Act
            invoice_withIncludeProperties = await unitOfWork.Invoice.GetFirstOrDefaultAsync(p => p.Amount.Equals(3000), includeProperties: "Customer");
            #endregion

            #region Assert
            Assert.NotNull(invoice_withIncludeProperties.Customer);
            Assert.Equal("name1", invoice_withIncludeProperties.Customer.FirstName);
            #endregion
        }

        [Fact]
        public async Task Remove_ShouldRemoveTheInvoice()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "Remove_ShouldRemoveTheInvoice").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            Invoice invoice = new()
            {
                Id = Guid.NewGuid(),
                Amount = 1000
            };
            await unitOfWork.Invoice.AddAsync(invoice);
            await unitOfWork.SaveAsync();
            Invoice removedInvoice = null;
            #endregion


            #region Act
            unitOfWork.Invoice.Remove(invoice);
            await unitOfWork.SaveAsync();
            removedInvoice = await unitOfWork.Invoice.GetFirstOrDefaultAsync(p => p.Amount.Equals(invoice.Amount));
            #endregion

            #region Assert
            Assert.Null(removedInvoice);
            #endregion

        }

        [Fact]
        public async Task RemoveRange_ShouldRemoveRangeOfInvoices()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RemoveRange_ShouldRemoveRangeOfInvoices").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            List<Invoice> invoices = new()
            {
                new Invoice()
                {
                    Id = Guid.NewGuid(),
                    Amount = 1000
                },
                new Invoice()
                {
                    Id = Guid.NewGuid(),
                    Amount = 2000
                },
                new Invoice()
                {
                    Id = Guid.NewGuid(),
                    Amount = 3000
                }
            };
            await unitOfWork.Invoice.AddAsync(invoices[0]);
            await unitOfWork.Invoice.AddAsync(invoices[1]);
            await unitOfWork.Invoice.AddAsync(invoices[2]);
            await unitOfWork.SaveAsync();
            IEnumerable<Invoice> removedInvoices = null;
            #endregion


            #region Act
            unitOfWork.Invoice.RemoveRange(invoices);
            await unitOfWork.SaveAsync();
            removedInvoices = await unitOfWork.Invoice.GetAllAsync();
            #endregion

            #region Assert
            Assert.Empty(removedInvoices);
            #endregion
        }

        [Fact]
        public async Task SoftRemoveAsync_ShouldChangeStatusToDeletedForInvoice()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "SoftRemoveAsync_ShouldChangeStatusToDeletedForInvoice").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            Invoice invoice = new()
            {
                Id = Guid.NewGuid(),
                Amount = 1000
            };
            await unitOfWork.Invoice.AddAsync(invoice);
            await unitOfWork.SaveAsync();
            Invoice softRemovedInvoice = null;
            #endregion


            #region Act
            await unitOfWork.Invoice.SoftRemoveAsync(invoice);
            await unitOfWork.SaveAsync();
            softRemovedInvoice = await unitOfWork.Invoice.GetFirstOrDefaultAsync(p => p.Amount.Equals(invoice.Amount));
            #endregion

            #region Assert
            Assert.Null(softRemovedInvoice);
            Assert.Equal(StatusEnum.Deleted, invoice.Status);
            #endregion

        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTheInvoice()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "UpdateAsync_ShouldUpdateTheInvoice").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            Invoice invoice = null;
            Guid guid = Guid.NewGuid();
            await unitOfWork.Invoice.AddAsync(new()
            {
                Id = guid,
                Amount = 1000
            });
            await unitOfWork.SaveAsync();
            Invoice updatedInvoice = null;
            #endregion


            #region Act
            invoice = await unitOfWork.Invoice.GetFirstOrDefaultAsync(p => p.Amount.Equals(1000));
            invoice.Amount = 2000;
            await unitOfWork.Invoice.UpdateAsync(invoice);
            await unitOfWork.SaveAsync();
            updatedInvoice = await unitOfWork.Invoice.GetFirstOrDefaultAsync(p => p.Id.Equals(guid));
            #endregion

            #region Assert
            Assert.Equal(2000, updatedInvoice.Amount);
            #endregion

        }


    }
}
