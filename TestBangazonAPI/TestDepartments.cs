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
                Department NewDepartment = JsonConvert.DeserializeObject<Department>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(department.Name, NewDepartment.Name);
                Assert.Equal(department.ExpenseBudget, NewDepartment.ExpenseBudget);
            }
        }
        [Fact]
        public async Task Test_Modify_Department()
        {
            // New last name to change to and test
            string NewName = "PutTest";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Department ModifiedDepartment = new Department
                {
                    Id = 1,
                    Name = NewName,
                    ExpenseBudget = 999999
                };
                var ModifiedDepartmentAsJSON = JsonConvert.SerializeObject(ModifiedDepartment);

                var response = await client.PutAsync(
                    "/api/departments/1",
                    new StringContent(ModifiedDepartmentAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var GetDepartment = await client.GetAsync("/api/departments/1");
                GetDepartment.EnsureSuccessStatusCode();

                string GetDepartmentBody = await GetDepartment.Content.ReadAsStringAsync();
                Department NewDepartment = JsonConvert.DeserializeObject<Department>(GetDepartmentBody);

                Assert.Equal(HttpStatusCode.OK, GetDepartment.StatusCode);
                Assert.Equal(NewName, NewDepartment.Name);
            }
        }
    }
}
