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
    public class UnitOfWork_Customer_Test
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
        public async Task AddAsync_ShouldAddNewCustomer()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "AddAsync_ShouldAddNewCustomer").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            Customer customer = null;
            #endregion


            #region Act
            await unitOfWork.Customer.AddAsync(new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = "name1",
                LastName = "family1"
            });
            await unitOfWork.SaveAsync();
            customer = await unitOfWork.Customer.GetFirstOrDefaultAsync(p => p.FirstName == "name1");
            #endregion

            #region Assert
            Assert.Equal("family1", customer.LastName);
            #endregion

        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCustomers()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "GetAllAsync_ShouldReturnAllCustomers").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.Customer.AddAsync(new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = "name1",
                LastName = "family1"
            });
            await unitOfWork.Customer.AddAsync(new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = "name2",
                LastName = "family2"
            });
            await unitOfWork.Customer.AddAsync(new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = "name3",
                LastName = "family3"
            });
            await unitOfWork.SaveAsync();
            IEnumerable<Customer> all_customers = null;
            #endregion


            #region Act
            all_customers = await unitOfWork.Customer.GetAllAsync();
            #endregion

            #region Assert
            Assert.Equal(3, all_customers.Count());
            #endregion
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCustomers_WithSpecificFilter()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "GetAllAsync_ShouldReturnAllCustomers_WithSpecificFilter").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.Customer.AddAsync(new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = "name1",
                LastName = "family1"
            });
            await unitOfWork.Customer.AddAsync(new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = "name2",
                LastName = "family2"
            });
            await unitOfWork.Customer.AddAsync(new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = "name3",
                LastName = "family2"
            });
            await unitOfWork.SaveAsync();
            IEnumerable<Customer> all_customers_withSpesificFilter = null;
            #endregion


            #region Act
            all_customers_withSpesificFilter = await unitOfWork.Customer.GetAllAsync(p => p.LastName.Equals("family2"));
            #endregion

            #region Assert
            Assert.Equal(2, all_customers_withSpesificFilter.Count());
            Assert.Equal("name3", all_customers_withSpesificFilter.ElementAt(1).FirstName);
            #endregion
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCustomers_WithIncludedProperty()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "GetAllAsync_ShouldReturnAllCustomers_WithIncludedProperty").Options;
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
            IEnumerable<Customer> all_customers_withIncludeProperties = null;
            #endregion


            #region Act
            all_customers_withIncludeProperties = await unitOfWork.Customer.GetAllAsync(includeProperties: "InvoiceList");
            #endregion

            #region Assert
            Assert.NotNull(all_customers_withIncludeProperties.FirstOrDefault().InvoiceList);
            Assert.Equal(3, all_customers_withIncludeProperties.FirstOrDefault().InvoiceList.Count());
            Assert.Equal(3000, all_customers_withIncludeProperties.FirstOrDefault().InvoiceList[2].Amount);
            #endregion
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_ShouldReturnOneCustomer_WithSpecificFilter()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "GetFirstOrDefaultAsync_ShouldReturnOneCustomer_WithSpecificFilter").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            await unitOfWork.Customer.AddAsync(new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = "name1",
                LastName = "family1"
            });
            await unitOfWork.Customer.AddAsync(new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = "name2",
                LastName = "family2"
            });
            await unitOfWork.Customer.AddAsync(new Customer()
            {
                Id = Guid.NewGuid(),
                FirstName = "name3",
                LastName = "family2"
            });
            await unitOfWork.SaveAsync();
            Customer customer_withSpesificFilter = null;
            #endregion


            #region Act
            customer_withSpesificFilter = await unitOfWork.Customer.GetFirstOrDefaultAsync(p => p.LastName.Equals("family2"));
            #endregion

            #region Assert
            Assert.NotNull(customer_withSpesificFilter);
            Assert.Equal("name2", customer_withSpesificFilter.FirstName);
            #endregion
        }

        [Fact]
        public async Task GetFirstOrDefaultAsync_ShouldReturnOneCustomer_WithIncludedProperty()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "GetFirstOrDefaultAsync_ShouldReturnOneCustomer_WithIncludedProperty").Options;
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
            Customer customer_withIncludeProperties = null;
            #endregion


            #region Act
            customer_withIncludeProperties = await unitOfWork.Customer.GetFirstOrDefaultAsync(p => p.FirstName.Equals("name1"), includeProperties: "InvoiceList");
            #endregion

            #region Assert
            Assert.NotNull(customer_withIncludeProperties.InvoiceList);
            Assert.Equal(3, customer_withIncludeProperties.InvoiceList.Count());
            Assert.Equal(2000, customer_withIncludeProperties.InvoiceList[1].Amount);
            #endregion
        }

        [Fact]
        public async Task Remove_ShouldRemoveTheCustomer()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "Remove_ShouldRemoveTheCustomer").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            Customer customer = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "name1",
                LastName = "family1"
            };
            await unitOfWork.Customer.AddAsync(customer);
            await unitOfWork.SaveAsync();
            Customer removedCustomer = null;
            #endregion


            #region Act
            unitOfWork.Customer.Remove(customer);
            await unitOfWork.SaveAsync();
            removedCustomer = await unitOfWork.Customer.GetFirstOrDefaultAsync(p => p.LastName.Equals(customer.LastName));
            #endregion

            #region Assert
            Assert.Null(removedCustomer);
            #endregion

        }

        [Fact]
        public async Task RemoveRange_ShouldRemoveRangeOfCustomers()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "RemoveRange_ShouldRemoveRangeOfCustomers").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            List<Customer> customers = new()
            {
                new Customer()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "name1",
                    LastName = "family1"
                },
                new Customer()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "name2",
                    LastName = "family2"
                },
                new Customer()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "name3",
                    LastName = "family3"
                }
            };
            await unitOfWork.Customer.AddAsync(customers[0]);
            await unitOfWork.Customer.AddAsync(customers[1]);
            await unitOfWork.Customer.AddAsync(customers[2]);
            await unitOfWork.SaveAsync();
            IEnumerable<Customer> removedCustomers = null;
            #endregion


            #region Act
            unitOfWork.Customer.RemoveRange(customers);
            await unitOfWork.SaveAsync();
            removedCustomers = await unitOfWork.Customer.GetAllAsync();
            #endregion

            #region Assert
            Assert.Empty(removedCustomers);
            #endregion
        }

        [Fact]
        public async Task SoftRemoveAsync_ShouldChangeStatusToDeletedForCustomer()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "SoftRemoveAsync_ShouldChangeStatusToDeletedForCustomer").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            Customer customer = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "name1",
                LastName = "family1"
            };
            await unitOfWork.Customer.AddAsync(customer);
            await unitOfWork.SaveAsync();
            Customer softRemovedCustomer = null;
            #endregion


            #region Act
            await unitOfWork.Customer.SoftRemoveAsync(customer);
            await unitOfWork.SaveAsync();
            softRemovedCustomer = await unitOfWork.Customer.GetFirstOrDefaultAsync(p => p.LastName.Equals(customer.LastName));
            #endregion

            #region Assert
            Assert.Null(softRemovedCustomer);
            Assert.Equal(StatusEnum.Deleted, customer.Status);
            #endregion

        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTheCustomer()
        {
            #region Arrange
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "UpdateAsync_ShouldUpdateTheCustomer").Options;
            dbContext = new ApplicationDbContext(options);
            unitOfWork = new UnitOfWork(dbContext);
            Customer customer = null;
            await unitOfWork.Customer.AddAsync(new()
            {
                Id = Guid.NewGuid(),
                FirstName = "name1",
                LastName = "family1"
            });
            await unitOfWork.SaveAsync();
            Customer updatedCustomer = null;
            #endregion


            #region Act
            customer = await unitOfWork.Customer.GetFirstOrDefaultAsync(p => p.LastName.Equals("family1"));
            customer.FirstName = "firsname1";
            customer.Email = "firstname1@family1.com";
            await unitOfWork.Customer.UpdateAsync(customer);
            await unitOfWork.SaveAsync();
            updatedCustomer = await unitOfWork.Customer.GetFirstOrDefaultAsync(p => p.LastName.Equals("family1"));
            #endregion

            #region Assert
            Assert.Equal("firsname1", updatedCustomer.FirstName);
            Assert.Equal("firstname1@family1.com", updatedCustomer.Email);
            #endregion

        }


    }
}
