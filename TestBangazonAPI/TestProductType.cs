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
    public class TestProductType
    {
        [Fact]
        public async Task Test_Get_All_ProductTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/producttype");
                string responseBody = await response.Content.ReadAsStringAsync();
                var productType = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productType.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/producttype/2");
                string responseBody = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<ProductType>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotNull(customer);
            }
        }

        [Fact]
        public async Task Test_Create_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                ProductType berries = new ProductType
                {
                    Name = "Berries"
                };
                var ProductAsJSON = JsonConvert.SerializeObject(berries);
                var response = await client.PostAsync(
                    "/api/producttype",
                    new StringContent(ProductAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();
                var NewProductType = JsonConvert.DeserializeObject<ProductType>(responseBody);
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(berries.Name, NewProductType.Name);
            }
        }

        [Fact]
        public async Task Test_Modify_ProductType()
        {
            // New last name to change to and test
            string newType = "Clean";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                ProductType modifiedProductType = new ProductType
                {
                    Name = newType,
 
                };
                var modifiedProudctTypeAsJSON = JsonConvert.SerializeObject(modifiedProductType);

                var response = await client.PutAsync(
                    "/api/productType/4",
                    new StringContent(modifiedProudctTypeAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getCleaning = await client.GetAsync("/api/productType/4");
                getCleaning.EnsureSuccessStatusCode();

                string getCleaningBody = await getCleaning.Content.ReadAsStringAsync();
                ProductType newCleaning = JsonConvert.DeserializeObject<ProductType>(getCleaningBody);

                Assert.Equal(HttpStatusCode.OK, getCleaning.StatusCode);
                Assert.Equal(newType, newCleaning.Name );
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                ProductType Brushes = new ProductType
                {
                    Name = "Bush Blue"
                };
                var BrushesAsJSON = JsonConvert.SerializeObject(Brushes);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/producttype",
                    new StringContent(BrushesAsJSON, Encoding.UTF8, "application/json")
                );


                string responseBody = await response.Content.ReadAsStringAsync();
                var NewBrush = JsonConvert.DeserializeObject<ProductType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(Brushes.Name, NewBrush.Name);
                /*
                    ACT
                */
                var deleteResponse = await client.DeleteAsync($"/api/ptoducttype/{NewBrush.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }
    }
}
