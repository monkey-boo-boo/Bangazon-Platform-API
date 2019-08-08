using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingProgramController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TrainingProgramController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string completed)
        {
            string SqlCommandText = @"SELECT tp.Id, tp.[Name], tp.StartDate, tp.EndDate, tp.MaxAttendees, e.FirstName, e.LastName FROM TrainingProgram tp
                                        JOIN EmployeeTraining et ON et.TrainingProgramId = tp.id
                                        JOIN Employee e ON e.Id = et.EmployeeId;";
            if (completed == "false")
            {
                SqlCommandText = @"SELECT tp.Id, tp.[Name], tp.StartDate, tp.EndDate, tp.MaxAttendees, e.FirstName, e.LastName FROM TrainingProgram tp
                                        JOIN EmployeeTraining et ON et.TrainingProgramId = tp.id
                                        JOIN Employee e ON e.Id = et.EmployeeId
                                        WHERE tp.EndDate > CURRENT_TIMESTAMP;";
            }
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SqlCommandText;

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {

                        int Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        string Name = reader.GetString(reader.GetOrdinal("Name"));
                        DateTime StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate"));
                        DateTime EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));
                        int MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"));
                        string FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                        string LastName = reader.GetString(reader.GetOrdinal("LastName"));
                        // You might have more columns

                        Employee employee = new Employee()
                        {
                            FirstName = FirstName,
                            LastName = LastName
                        };

                        if (!trainingPrograms.Any(tp => tp.Id == Id))
                        {
                            TrainingProgram trainingProgram = new TrainingProgram()
                            {
                                Id = Id,
                                Name = Name,
                                StartDate = StartDate,
                                EndDate = EndDate,
                                MaxAttendees = MaxAttendees,
                                EmployeesInProgram = new List<Employee>()
                            };

                            trainingProgram.EmployeesInProgram.Add(employee);
                            trainingPrograms.Add(trainingProgram);
                        }
                        else
                        {
                            var findProgram = trainingPrograms.Find(tp => tp.Id == Id);
                            findProgram.EmployeesInProgram.Add(employee);
                        }
                    }

                    reader.Close();

                    return Ok(trainingPrograms);
                }
            }
        }


        // GET api/values/5
        [HttpGet("{id}", Name = "GetTrainingProgram")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT [Id], [StartDate], [EndDate], [MaxAttendees], [Name]
                                        FROM TrainingProgram
                                        WHERE TrainingProgram.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    TrainingProgram trainingProgram = null;
                    if (reader.Read())
                    {
                        trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),
                        };
                    }

                    reader.Close();

                    return Ok(trainingProgram);
                }
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TrainingProgram trainingProgram)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                       INSERT INTO TrainingProgram ( [StartDate], [EndDate], [MaxAttendees], [Name])
                       OUTPUT INSERTED.Id
                       VALUES (@StartDate, @EndDate, @MaxAttendees, @Name)
                   ";
                    cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));
                    cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                    trainingProgram.Id = (int)await cmd.ExecuteScalarAsync();
                    return CreatedAtRoute("GetTrainingProgram", new { id = trainingProgram.Id }, trainingProgram);
                }
            }
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute]int id, [FromBody] TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE TrainingProgram
                            SET [Name]  = @Name
                            StartDate = @StartDate
                            EndDate = @EndDate
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));
                        cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!TrainingProgramExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {


            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT Id, [Name], StartDate, EndDate, MaxAttendees 
                                        FROM TrainingProgram 
                                        WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        TrainingProgram trainingProgram = null;
                        if (reader.Read())
                        {
                            trainingProgram = new TrainingProgram
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("NAme")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                                // You might have more columns
                            };
                        }

                        reader.Close();

                        if (trainingProgram.StartDate > DateTime.Now)
                        {
                            using (SqlConnection conn2 = Connection)
                            {
                                conn2.Open();
                                using (SqlCommand cmd2 = conn2.CreateCommand())
                                {
                                    cmd2.CommandText = @"DELETE FROM EmployeeTraining 
                                                         WHERE TrainingProgramId = @id
                                                         DELETE FROM TrainingProgram
                                                         WHERE Id = @id
                                                          ";
                                    cmd2.Parameters.Add(new SqlParameter("@id", id));

                                    int rowsAffected = cmd2.ExecuteNonQuery();
                                    if (rowsAffected > 0)
                                    {
                                        return new StatusCodeResult(StatusCodes.Status204NoContent);
                                    }
                                    throw new Exception("No rows affected");
                                }
                            }

                        }
                        else
                        {
                            return StatusCode(403);
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (!TrainingProgramExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }

            }
        }


        private bool TrainingProgramExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM TrainingProgram WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
