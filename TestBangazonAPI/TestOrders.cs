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
                var response = await client.GetAsync("/api/orders/1");


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
                //Assert.Equal(CreatedOrder.CustomerId, NewCreatedOrder.CustomerId);
                //Assert.Equal(CreatedOrder.PaymentTypeId, NewCreatedOrder.PaymentTypeId);
            }
        }
        [Fact]
        public async Task Test_Modify_Order()
        {
            int newCustomerId = 5;

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */

                Order order = new Order
                {
                    Id = 2,
                    CustomerId = newCustomerId,
                    PaymentTypeId = 1
                };
                var OrderAsJSON = JsonConvert.SerializeObject(order);

                var response = await client.PutAsync(
                    "/api/orders/2",
                    new StringContent(OrderAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                */

                var GetOrder = await client.GetAsync("/api/orders/2");
                GetOrder.EnsureSuccessStatusCode();

                string GetOrderBody = await GetOrder.Content.ReadAsStringAsync();
                Order newOrder = JsonConvert.DeserializeObject<Order>(GetOrderBody);

                Assert.Equal(HttpStatusCode.OK, GetOrder.StatusCode);
                Assert.Equal(newCustomerId, newOrder.CustomerId);
            }
        }
    }
}
