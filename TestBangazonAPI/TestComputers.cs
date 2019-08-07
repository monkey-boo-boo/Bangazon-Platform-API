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
    public class TestComputers
    {
        [Fact]
        public async Task Test_Get_All_Computers()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ACT
                */
                var response = await client.GetAsync("/api/computers");


                string responseBody = await response.Content.ReadAsStringAsync();
                var computers = JsonConvert.DeserializeObject<List<Computer>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(computers.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Computer_By_Id()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ACT
                */
                var response = await client.GetAsync("/api/computers/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var computer = JsonConvert.DeserializeObject<Computer>(responseBody);


                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("GFE-744", computer.Make);
                Assert.Equal("Zoombeat", computer.Manufacturer);
                Assert.NotNull(computer);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_Computer()
        {
            var purchased = new DateTime(2008, 10, 04);
            var decommissioned = new DateTime(2008, 10, 22);

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                Computer MacbookPro = new Computer
                {
                    PurchaseDate = purchased,
                    DecomissionDate = decommissioned,
                    Make = "MacbookPro",
                    Manufacturer = "Apple"
                };
                var MacAsJSON = JsonConvert.SerializeObject(MacbookPro);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/computers",
                    new StringContent(MacAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var NewMacbook = JsonConvert.DeserializeObject<Computer>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(MacbookPro.Make, NewMacbook.Make);
                Assert.Equal(MacbookPro.Manufacturer, NewMacbook.Manufacturer);

                var deleteResponse = await client.DeleteAsync($"/api/computers/{NewMacbook.Id}");

                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Computer()
        {
            var purchased = new DateTime(2015, 08, 11);
            var newDecommissioned = new DateTime(2016, 10, 22);

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT
                */

                Computer modifiedComputer = new Computer
                {
                    Id = 2,
                    PurchaseDate = purchased,
                    DecomissionDate = newDecommissioned,
                    Make = "YRV - 483",
                    Manufacturer = "Oyondu"
                };
                var modifiedComputerAsJSON = JsonConvert.SerializeObject(modifiedComputer);

                var response = await client.PutAsync(
                    "/api/computers/2",
                    new StringContent(modifiedComputerAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET
                */
                var GetComputer = await client.GetAsync("/api/computers/2");
                GetComputer.EnsureSuccessStatusCode();

                string GetBody = await GetComputer.Content.ReadAsStringAsync();
                Computer NewComputer = JsonConvert.DeserializeObject<Computer>(GetBody);

                Assert.Equal(HttpStatusCode.OK, GetComputer.StatusCode);
                Assert.Equal(newDecommissioned, NewComputer.DecomissionDate);
            }
        }
    }
}
