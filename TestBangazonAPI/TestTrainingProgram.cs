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
    public class TestTrainingProgram
    {
        [Fact]
        public async Task Test_Get_All_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/trainingprogram");
                string responseBody = await response.Content.ReadAsStringAsync();
                var productType = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productType.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/trainingprogram/2");
                string responseBody = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.NotNull(customer);
            }
        }

        [Fact]
        public async Task Test_Create_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                TrainingProgram berries = new TrainingProgram
                {
                    Name = "Berries"
                };
                var ProductAsJSON = JsonConvert.SerializeObject(berries);
                var response = await client.PostAsync(
                    "/api/trainingprogram",
                    new StringContent(ProductAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();
                var NewTrainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(berries.Name, NewTrainingProgram.Name);
            }
        }

        [Fact]
        public async Task Test_Modify_TrainingProgram()
        {
            // New last name to change to and test
            string newType = "Clean";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                */
                TrainingProgram modifiedTrainingProgram = new TrainingProgram
                {
                    Name = newType,
 
                };
                var modifiedProudctTypeAsJSON = JsonConvert.SerializeObject(modifiedTrainingProgram);

                var response = await client.PutAsync(
                    "/api/productType/3",
                    new StringContent(modifiedProudctTypeAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                /*
                    GET section
                    Verify that the PUT operation was successful
                */
                var getCleaning = await client.GetAsync("/api/productType/3");
                getCleaning.EnsureSuccessStatusCode();

                string getCleaningBody = await getCleaning.Content.ReadAsStringAsync();
                TrainingProgram newCleaning = JsonConvert.DeserializeObject<TrainingProgram>(getCleaningBody);

                Assert.Equal(HttpStatusCode.OK, getCleaning.StatusCode);
                Assert.Equal(newType, newCleaning.Name );
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                TrainingProgram Brushes = new TrainingProgram
                {
                    Name = "Bush Blue"
                };
                var BrushesAsJSON = JsonConvert.SerializeObject(Brushes);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/trainingprogram",
                    new StringContent(BrushesAsJSON, Encoding.UTF8, "application/json")
                );


                string responseBody = await response.Content.ReadAsStringAsync();
                var NewBrush = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(Brushes.Name, NewBrush.Name);
                /*
                    ACT
                */
                var deleteResponse = await client.DeleteAsync($"/api/trainingprogram/{NewBrush.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            }
        }
    }
}
