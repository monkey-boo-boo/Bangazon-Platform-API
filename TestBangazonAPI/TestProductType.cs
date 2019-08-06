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
        public async Task Test_Get_All_ProudctTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/ProductTypes");


                string responseBody = await response.Content.ReadAsStringAsync();
                var productTypes = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypes.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Modify_ProductType()
        {
            // New last name to change to and test
            string newType = "Cleaning";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                ProductType modifiedProductType = new ProductType
                {
                    Name = "Automotive",
 
                };
                var modifiedProudctTypeAsJSON = JsonConvert.SerializeObject(modifiedProductType);

                var response = await client.PutAsync(
                    "/api/productType/1",
                    new StringContent(modifiedProudctTypeAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getProductType = await client.GetAsync("/api/productType/1");
                getProductType.EnsureSuccessStatusCode();

                string getProductTypeBody = await getProductType.Content.ReadAsStringAsync();
                ProductType newProductType = JsonConvert.DeserializeObject<ProductType>(getProductTypeBody);

                Assert.Equal(HttpStatusCode.OK, getProductType.StatusCode);
                Assert.Equal(newType, newProductType.Name );
            }
        }
    }
}
