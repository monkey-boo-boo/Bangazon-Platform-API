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
    public class TestEmployees
    {
        [Fact]
        public async Task Test_Get_All_Employees()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ACT
                */
                
                var response = await client.GetAsync("/api/employees");


                string responseBody = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(employees.Count > 0);
            }
        }
        
        [Fact]
        public async Task Test_Get_Single_Employee()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/employees/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var employee = JsonConvert.DeserializeObject<Employee>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotNull(employee);
            }
        }

        [Fact]
        public async Task Test_Create_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                Employee Gob = new Employee
                {
                    FirstName = "Gob",
                    LastName = "Bleuth",
                    DepartmentId = 2,
                    DepartmentName = "Magic",
                    IsSuperVisor = false
                };
                var GobAsJSON = JsonConvert.SerializeObject(Gob);


                var response = await client.PostAsync(
                    "/api/employees",
                    new StringContent(GobAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var NewGob = JsonConvert.DeserializeObject<Customer>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(Gob.FirstName, NewGob.FirstName);
                Assert.Equal(Gob.LastName, NewGob.LastName);
            }
        }

        [Fact]
        public async Task Test_Modify_Employee()
        {
            string NewFirstName = "Philbert";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                 
                Employee Philbert = new Employee
                {
                    Id = 3,
                    FirstName = NewFirstName,
                    LastName = "Stirgess",
                    DepartmentId = 2,
                    DepartmentName = "Sales",
                    IsSuperVisor = false
                };
                var PhilbertAsJSON = JsonConvert.SerializeObject(Philbert);

                var response = await client.PutAsync(
                    "/api/employees/3",
                    new StringContent(PhilbertAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                 
                var GetEmployee = await client.GetAsync("/api/employees/3");
                GetEmployee.EnsureSuccessStatusCode();

                string GetEmployeeBody = await GetEmployee.Content.ReadAsStringAsync();
                Employee NewPhilbert = JsonConvert.DeserializeObject<Employee>(GetEmployeeBody);

                Assert.Equal(HttpStatusCode.OK, GetEmployee.StatusCode);
                Assert.Equal(NewFirstName, NewPhilbert.FirstName);
            }
        }
    }
}

