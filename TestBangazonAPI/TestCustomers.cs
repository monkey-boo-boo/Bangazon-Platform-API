using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TestBangazonAPI
{
    public class TestCustomers
    {
        [Fact]
        public async Task Test_Get_All_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var response = await client.GetAsync("/api/departments");


                string responseBody = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(departments.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Get_Single_Customer()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/customers/2");


                string responseBody = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<Customer>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotNull(customer);
            }
        }
        [Fact]
        public async Task Test_Get_NonExitant_Customer_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/customer/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task Test_Create_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                Customer Berry = new Customer
                {
                    FirstName = "John",
                    LastName = "Doe"
                };
                var BerryAsJSON = JsonConvert.SerializeObject(Berry);


                var response = await client.PostAsync(
                    "/api/customers",
                    new StringContent(BerryAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var NewBerry = JsonConvert.DeserializeObject<Customer>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(Berry.FirstName, NewBerry.FirstName);
                Assert.Equal(Berry.LastName, NewBerry.LastName);
            }
        }
        [Fact]
        public async Task Test_Modify_Customer()
        {
            // New last name to change to and test
            string NewFirstName = "Jimmy";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Customer ModifiedCustomer = new Customer
                {
                    Id = 1,
                    FirstName = NewFirstName,
                    LastName = "Buffet"
                };
                var ModifiedButterAsJSON = JsonConvert.SerializeObject(ModifiedCustomer);

                var response = await client.PutAsync(
                    "/api/customers/1",
                    new StringContent(ModifiedButterAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var GetBuffet = await client.GetAsync("/api/customers/1");
                GetBuffet.EnsureSuccessStatusCode();

                string GetBuffetBody = await GetBuffet.Content.ReadAsStringAsync();
                Customer NewBuffet = JsonConvert.DeserializeObject<Customer>(GetBuffetBody);

                Assert.Equal(HttpStatusCode.OK, GetBuffet.StatusCode);
                Assert.Equal(NewFirstName, NewBuffet.FirstName);
            }
        }
    }
}
