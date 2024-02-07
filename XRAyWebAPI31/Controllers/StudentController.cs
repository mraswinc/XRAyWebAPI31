using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using XRAyWebAPI31.Models;

namespace XRAyWebAPI31.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        // Create a GET method that returns the data as JSON
        [HttpGet]
        public ActionResult<List<Student>> GetData()
        {
            // Call the GetDataSet method and store the result in a variable
            var data = GetStudents();

            // Return the data as JSON
            return Ok(data);
        }

        private List<Student> GetStudents()
        {
            string connectionString = @"data source={Server}; database=StudentDB; integrated security=SSPI";
            // Create a SqlConnection object

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create a SqlDataAdapter object
                SqlDataAdapter adapter = new SqlDataAdapter();

                // Set the SelectCommand property to a SqlCommand object with the stored procedure name
                adapter.SelectCommand = new SqlCommand("spGetStudents", connection);

                // Set the CommandType property to CommandType.StoredProcedure
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;

                // Create a DataSet object
                DataSet dataSet = new DataSet();

                // Fill the DataSet with the data from the data source
                adapter.Fill(dataSet);
                var students = new List<Student>();

                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        var student = new Student
                        {
                            Id = (int)row["ID"],
                            Name = (string)row["Name"],
                            Email = (string)row["Email"],
                            Mobile = (string)row["Mobile"]
                        };
                        students.Add(student);
                    }
                }

                // Return the DataSet
                return students;
            }
        }
    }
}
