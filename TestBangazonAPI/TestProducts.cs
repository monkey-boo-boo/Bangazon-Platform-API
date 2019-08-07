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
    public class TestProducts
    {
        [Fact]
        public async Task Test_Get_All_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/products");


                string responseBody = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(products.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Product_By_Id()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/products/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(responseBody);


                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Honey - Liquid", product.Title);
                Assert.Equal("ligula suspendisse ornare consequat lectus in est", product.Description);
                Assert.Equal(25, product.Quantity);
                Assert.Equal(1, product.ProductTypeId);
                Assert.Equal(2, product.CustomerId);
                Assert.NotNull(product);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                Product anotherProduct = new Product
                {
                    CustomerId = 1,
                    ProductTypeId = 1,
                    Price = 1.7600m,
                    Title = "Title1",
                    Description = "Description1 lalala dodo doo",
                    Quantity = 2
                };
                var productAsJSON = JsonConvert.SerializeObject(anotherProduct);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/products",
                    new StringContent(productAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var NewProduct = JsonConvert.DeserializeObject<Product>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(anotherProduct.Title, NewProduct.Title);
                Assert.Equal(anotherProduct.Description, NewProduct.Description);

                var deleteResponse = await client.DeleteAsync($"/api/products/{NewProduct.Id}");

                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Product()
        {
            string newTitle = "newTitle";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT
                */

                Product modifiedProduct = new Product
                {
                    Id = 2,
                    CustomerId = 2,
                    ProductTypeId = 1,
                    Price = 1.7600m,
                    Quantity = 32,
                    Title = newTitle,
                    Description = "tempus sit amet sem fusce consequat nulla"
                };
                var modifiedProductAsJSON = JsonConvert.SerializeObject(modifiedProduct);

                var response = await client.PutAsync(
                    "/api/products/2",
                    new StringContent(modifiedProductAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET
                */
                var GetProduct = await client.GetAsync("/api/products/2");
                GetProduct.EnsureSuccessStatusCode();

                string GetProductBody = await GetProduct.Content.ReadAsStringAsync();
                Product NewProduct = JsonConvert.DeserializeObject<Product>(GetProductBody);

                Assert.Equal(HttpStatusCode.OK, GetProduct.StatusCode);
                Assert.Equal(newTitle, NewProduct.Title);
            }
        }
    }
}
