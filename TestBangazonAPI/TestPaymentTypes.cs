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
    public class TestPaymentTypes
    {
        [Fact]
        public async Task Test_Get_All_PaymentTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/paymenttypes");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentTypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymentTypes.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Single_PaymentType()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var response = await client.GetAsync("/api/paymenttypes/3");


                string responseBody = await response.Content.ReadAsStringAsync();
                var payment = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("diners-club-enroute", payment.Name);
                Assert.Equal(16797, payment.AccountNumber);
                Assert.Equal(3, payment.Id);
                Assert.NotNull(payment);
            }
        }
        [Fact]
        public async Task Test_Create_Delete_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                PaymentType Visa = new PaymentType
                {
                    Name = "Visa",
                    AccountNumber = 555666,
                    CustomerId = 2
                };
                var VisaAsJSON = JsonConvert.SerializeObject(Visa);
                var response = await client.PostAsync(
                    "/api/paymenttypes",
                    new StringContent(VisaAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();
                var NewVisa = JsonConvert.DeserializeObject<PaymentType>(responseBody);
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(Visa.Name, NewVisa.Name);
                Assert.Equal(Visa.AccountNumber, NewVisa.AccountNumber);

                var deleteResponse = await client.DeleteAsync($"/api/paymenttypes/{NewVisa.Id}");

                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }
        [Fact]
        public async Task Test_Modify_PaymentType()
        {
            // New last name to change to and test
            string NewName = "MasterCard";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                PaymentType ModifiedPaymentType = new PaymentType
                {
                    Id = 1,
                    AccountNumber = 12769,
                    Name = NewName
                };
                var ModifiedPaymentTypeAsJSON = JsonConvert.SerializeObject(ModifiedPaymentType);

                var response = await client.PutAsync(
                    "/api/paymenttypes/1",
                    new StringContent(ModifiedPaymentTypeAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var GetPaymentType = await client.GetAsync("/api/paymenttypes/1");
                GetPaymentType.EnsureSuccessStatusCode();

                string PaymentTypeBody = await GetPaymentType.Content.ReadAsStringAsync();
                PaymentType NewPaymentType = JsonConvert.DeserializeObject<PaymentType>(PaymentTypeBody);

                Assert.Equal(HttpStatusCode.OK, GetPaymentType.StatusCode);
                Assert.Equal(NewName, NewPaymentType.Name);
            }
        }
    }
}
