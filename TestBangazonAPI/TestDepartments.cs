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
    public class TestDepartments
    {
        [Fact]
        public async Task Test_Get_All_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var response = await client.GetAsync("/api/customers");


                string responseBody = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customers.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Get_Single_Customer()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/departments/2");


                string responseBody = await response.Content.ReadAsStringAsync();
                var department = JsonConvert.DeserializeObject<Department>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotNull(department);
            }
        }
        //[Fact]
        //public async Task Test_Get_NonExitant_Customer_Fails()
        //{

        //    using (var client = new APIClientProvider().Client)
        //    {
        //        var response = await client.GetAsync("/api/departments/999999999");
        //        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        //    }
        //}
        [Fact]
        public async Task Test_Create_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                Department department = new Department

                {
                    Name = "TestDepartment",
                    ExpenseBudget = 9999999
                };
                var DepartmentAsJson = JsonConvert.SerializeObject(department);


                var response = await client.PostAsync(
                    "/api/departments",
                    new StringContent(DepartmentAsJson, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var NewDepartment = JsonConvert.DeserializeObject<Department>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(department.Name, NewDepartment.Name);
                Assert.Equal(department.ExpenseBudget, NewDepartment.ExpenseBudget);
            }
        }
        //[Fact]
        //public async Task Test_Modify_Customer()
        //{
        //    // New last name to change to and test
        //    string NewFirstName = "Jimmy";

        //    using (var client = new APIClientProvider().Client)
        //    {
        //        /*
        //            PUT section
        //         */
        //        Customer ModifiedCustomer = new Customer
        //        {
        //            Id = 1,
        //            FirstName = NewFirstName,
        //            LastName = "Buffet"
        //        };
        //        var ModifiedButterAsJSON = JsonConvert.SerializeObject(ModifiedCustomer);

        //        var response = await client.PutAsync(
        //            "/api/customers/1",
        //            new StringContent(ModifiedButterAsJSON, Encoding.UTF8, "application/json")
        //        );
        //        string responseBody = await response.Content.ReadAsStringAsync();

        //        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        //        /*
        //            GET section
        //         */
        //        var GetBuffet = await client.GetAsync("/api/customers/1");
        //        GetBuffet.EnsureSuccessStatusCode();

        //        string GetBuffetBody = await GetBuffet.Content.ReadAsStringAsync();
        //        Customer NewBuffet = JsonConvert.DeserializeObject<Customer>(GetBuffetBody);

        //        Assert.Equal(HttpStatusCode.OK, GetBuffet.StatusCode);
        //        Assert.Equal(NewFirstName, NewBuffet.FirstName);
        //    }
        //}
    }
}
