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
    public class TestOrders
    {
        [Fact]
        public async Task Test_Get_All_Orders()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var response = await client.GetAsync("/api/orders");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Get_Single_Order()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/orders/2");


                string responseBody = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotNull(order);
            }
        }
        [Fact]
        public async Task Test_Get_NonExitant_Order_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/order/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task Test_Create_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                Order CreatedOrder = new Order
                {
                    CustomerId = 1,
                    PaymentTypeId = 1
                };
                var CreatedOrderAsJSON = JsonConvert.SerializeObject(CreatedOrder);


                var response = await client.PostAsync(
                    "/api/orders",
                    new StringContent(CreatedOrderAsJSON, Encoding.UTF8, "application/json")
                );
                
                string responseBody = await response.Content.ReadAsStringAsync();
                var NewCreatedOrder = JsonConvert.DeserializeObject<Order>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(CreatedOrder.CustomerId, NewCreatedOrder.CustomerId);
                Assert.Equal(CreatedOrder.PaymentTypeId, NewCreatedOrder.PaymentTypeId);
            }
        }
        [Fact]
        public async Task Test_Modify_Order()
        {
            // New last name to change to and test
            int NewCustomerId = 1;

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Order ModifiedOrder = new Order
                {
                    Id = 1,
                    CustomerId = NewCustomerId,
                    PaymentTypeId = 1
                };
                var ModifiedOrderAsJSON = JsonConvert.SerializeObject(ModifiedOrder);

                var response = await client.PutAsync(
                    "/api/orders/1",
                    new StringContent(ModifiedOrderAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var GetOrder = await client.GetAsync("/api/orders/1");
                GetOrder.EnsureSuccessStatusCode();

                string GetOrderBody = await GetOrder.Content.ReadAsStringAsync();
                Order NewOrder = JsonConvert.DeserializeObject<Order>(GetOrderBody);

                Assert.Equal(HttpStatusCode.OK, GetOrder.StatusCode);
                Assert.Equal(NewCustomerId, NewOrder.CustomerId);
            }
        }
    }
}
