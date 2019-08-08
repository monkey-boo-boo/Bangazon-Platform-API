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
            DateTime startDate = new DateTime(2020, 10, 10);
            DateTime endDate = new DateTime(2020, 11, 10);
            using (var client = new APIClientProvider().Client)
            {
                TrainingProgram trainingProgram = new TrainingProgram
                {
                    Name = "Training Day",
                    StartDate = startDate,
                    EndDate = endDate,
                    MaxAttendees = 60
                };
                var ProductAsJSON = JsonConvert.SerializeObject(trainingProgram);
                var response = await client.PostAsync(
                    "/api/trainingprogram",
                    new StringContent(ProductAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();
                var NewTrainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(trainingProgram.Name, NewTrainingProgram.Name);
                Assert.Equal(trainingProgram.StartDate, NewTrainingProgram.StartDate);
                Assert.Equal(trainingProgram.EndDate, NewTrainingProgram.EndDate);
                Assert.Equal(trainingProgram.MaxAttendees, NewTrainingProgram.MaxAttendees);
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
        public async Task Test_Delete_Future_Training_Programs()
        {


            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                TrainingProgram newTrainingProgram = new TrainingProgram()
                {
                    Name = "Sensitivity Training",
                    StartDate = new DateTime(2020, 10, 26),
                    EndDate = new DateTime(2021, 11, 15),
                    MaxAttendees = 60
                };

                var jsonTrainingProgram = JsonConvert.SerializeObject(newTrainingProgram);


                var response = await client.PostAsync(
                    "/api/trainingprogram",
                    new StringContent(jsonTrainingProgram, Encoding.UTF8, "application/json")
                    );


                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);


                /*
                    ACT
                */
                var deleteResponse = await client.DeleteAsync($"/api/trainingprogram/{trainingProgram.Id}");


                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_Past_Training_Programs()
        {


            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var deleteResponse = await client.DeleteAsync($"/api/trainingprogram/2");


                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
            }
        }
    }
}
