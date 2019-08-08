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
    public class EmployeesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
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
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id AS EmployeeId, e.FirstName, e.LastName, e.IsSuperVisor, e.DepartmentId, 
                                        d.[Name] AS DepartmentName,
                                        co.Id AS ComputerId, co.PurchaseDate, co.DecomissionDate, co.Make, co.Manufacturer
                                        FROM Employee e
                                        LEFT JOIN Department d ON d.Id = e.DepartmentId
                                        LEFT JOIN ComputerEmployee c ON c.EmployeeId = e.Id
                                        LEFT Join Computer co ON c.ComputerId = co.Id";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId")))
                        {
                            Computer computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                            {
                                computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                            };

                            employee.Computer = computer;
                        }

                        employees.Add(employee);
                    }

                    reader.Close();

                    return Ok(employees);
                }
            }
        }
        
        // GET api/values/5
        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> Get(int id)
        {

            //Add "if Id doesnt exist function" here

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT e.Id AS EmployeeId, e.FirstName, e.LastName, e.IsSuperVisor, e.DepartmentId, 
                                        d.[Name] AS DepartmentName,
                                        co.Id AS ComputerId, co.PurchaseDate, co.DecomissionDate, co.Make, co.Manufacturer
                                        FROM Employee e
                                        LEFT JOIN Department d ON d.Id = e.DepartmentId
                                        LEFT JOIN ComputerEmployee c ON c.EmployeeId = e.Id
                                        LEFT Join Computer co ON c.ComputerId = co.Id
                                        WHERE e.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Employee employee = null;
                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("ComputerId")))
                        {
                            Computer computer = new Computer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                Make = reader.GetString(reader.GetOrdinal("Make")),
                                Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                            {
                                computer.DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                            };

                            employee.Computer = computer;
                        }

                    }

                    reader.Close();

                    return Ok(employee);
                }
            }
        }
        
        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee employee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor)
                    OUTPUT INSERTED.Id
                    VALUES (@firstName, @lastName, @departmentId, @isSupervisor)
                    ";

                    cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                    cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSuperVisor));

                    employee.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetEmployee", new { id = employee.Id }, employee);
                }
            }
        }
        
        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Employee
                            SET FirstName = @firstName, LastName = @lastName, 
                            DepartmentId = @departmentId, IsSuperVisor = @isSupervisor
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", employee.Id));
                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSuperVisor));
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
                    if (!EmployeeExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
        }
        private bool EmployeeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM Employee WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}

