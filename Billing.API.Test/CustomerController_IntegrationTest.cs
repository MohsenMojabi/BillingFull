using Billing.API;
using Billing.DataAccess.Repository.IRepository;
using Billing.Models.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Billing.API.Test
{
    public class CustomerController_IntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _fixture;
        private readonly string url = "/api/Customer/";
        private readonly Guid guid = Guid.NewGuid(), guidWithInvoices = Guid.NewGuid();
        private Customer customer = null, customerWithInvoices = null;
        public CustomerController_IntegrationTest(CustomWebApplicationFactory fixture)
        {
            _fixture = fixture;
            HttpClient client = _fixture.CreateClient();

            customer = new()
            {
                Id = guid,
                FirstName = "customerName1",
                LastName = "customerFamily1"
            };
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
            _ = client.PostAsync(url, httpContent).GetAwaiter().GetResult();

            customerWithInvoices = new()
            {
                Id = guidWithInvoices,
                FirstName = "customerName1",
                LastName = "customerFamily1"
            };
            httpContent = new StringContent(JsonConvert.SerializeObject(customerWithInvoices), Encoding.UTF8, "application/json");
            _ = client.PostAsync(url, httpContent).GetAwaiter().GetResult();

            Invoice invoice = new()
            {
                CustomerId = guidWithInvoices,
                Amount = 1000
            };
            httpContent = new StringContent(JsonConvert.SerializeObject(invoice), Encoding.UTF8, "application/json");
            _ = client.PostAsync("/api/Invoice/", httpContent).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task Get_ShouldReturnAllCustomers()
        {
            #region Arrange
            HttpClient client = _fixture.CreateClient();
            #endregion

            #region Act
            HttpResponseMessage response = await client.GetAsync(url + "GetCustomers");
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(content);
            #endregion

            #region Assert
            Assert.NotNull(customers);
            #endregion
        }

        [Fact]
        public async Task Get_ShouldReturnAllCustomers_WithSearchedTerm()
        {
            #region Arrange
            HttpClient client = _fixture.CreateClient();
            #endregion

            #region Act
            HttpResponseMessage response = await client.GetAsync(url + "GetCustomers/" + "customerName1");
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(content);
            #endregion

            #region Assert
            Assert.NotNull(customers);
            Assert.Equal("customerName1", customers.FirstOrDefault().FirstName);
            #endregion

        }

        [Fact]
        public async Task Get_ShouldReturnOneCustomer_ById()
        {
            #region Arrange
            HttpClient client = _fixture.CreateClient();
            #endregion

            #region Act
            HttpResponseMessage response = await client.GetAsync(url + guid);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            Customer customer = JsonConvert.DeserializeObject<Customer>(content);
            #endregion

            #region Assert
            Assert.NotNull(customer);
            Assert.Equal("customerFamily1", customer.LastName);
            #endregion

        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_ForNonExistingCustomer()
        {
            #region Arrange
            HttpClient client = _fixture.CreateClient();
            #endregion

            #region Act
            HttpResponseMessage response = await client.GetAsync(url + Guid.NewGuid());
            string content = await response.Content.ReadAsStringAsync();
            #endregion

            #region Assert
            Assert.Equal("NotFound", response.StatusCode.ToString());
            Assert.Equal("Customer does not exist", content);
            #endregion

        }

        [Fact]
        public async Task Post_ShouldCreateCustomer()
        {
            #region Arrange
            HttpClient client = _fixture.CreateClient();
            Customer customer = new()
            {
                Id = Guid.NewGuid(),
                FirstName = "customerName2",
                LastName = "customerFamily2"
            };
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
            #endregion

            #region Act
            HttpResponseMessage response = await client.PostAsync(url, httpContent);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            #endregion

            #region Assert
            Assert.Equal("Customer created successfully", content);
            Assert.True(response.IsSuccessStatusCode);
            #endregion
        }

        [Fact]
        public async Task Put_ShouldUpdateCustomer()
        {
            #region Arrange
            HttpClient client = _fixture.CreateClient();
            customer.FirstName = "customerName3";
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
            #endregion

            #region Act
            HttpResponseMessage response = await client.PutAsync(url + guid, httpContent);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            #endregion

            #region Assert
            Assert.Equal("Customer updated successfully", content);
            Assert.True(response.IsSuccessStatusCode);
            #endregion
        }

        [Fact]
        public async Task Put_ShouldReturnNotFound_ForNonExistingCustomer()
        {
            #region Arrange
            HttpClient client = _fixture.CreateClient();
            customer.FirstName = "customerName3";
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
            #endregion

            #region Act
            HttpResponseMessage response = await client.PutAsync(url + Guid.NewGuid(), httpContent);
            string content = await response.Content.ReadAsStringAsync();
            #endregion

            #region Assert
            Assert.Equal("NotFound", response.StatusCode.ToString());
            Assert.Equal("Customer does not exist", content);
            #endregion
        }

        [Fact]
        public async Task Delete_ShouldDeleteCustomer()
        {
            #region Arrange
            HttpClient client = _fixture.CreateClient();
            #endregion

            #region Act
            HttpResponseMessage response = await client.DeleteAsync(url + guid);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            #endregion

            #region Assert
            Assert.Equal("Customer deleted successfully", content);
            Assert.True(response.IsSuccessStatusCode);
            #endregion
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_ForNonExistingCustomer()
        {
            #region Arrange
            HttpClient client = _fixture.CreateClient();
            #endregion

            #region Act
            HttpResponseMessage response = await client.DeleteAsync(url + Guid.NewGuid());
            string content = await response.Content.ReadAsStringAsync();
            #endregion

            #region Assert
            Assert.Equal("NotFound", response.StatusCode.ToString());
            Assert.Equal("Customer does not exist", content);
            #endregion
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_ForCustomerWithExistingInvoices()
        {
            #region Arrange
            HttpClient client = _fixture.CreateClient();
            #endregion

            #region Act
            HttpResponseMessage response = await client.DeleteAsync(url + guidWithInvoices);
            string content = await response.Content.ReadAsStringAsync();
            #endregion

            #region Assert
            Assert.Equal("BadRequest", response.StatusCode.ToString());
            Assert.Equal("This customer has some invoices. First delete them", content);
            #endregion
        }


    }
}
