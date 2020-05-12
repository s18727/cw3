using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using cw3.Models;
using Wyklad3.Services;
using cw3.DTOs;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Wyklad3.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {

        private readonly IDbService _dbService;
        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents([FromQuery]string orderBy)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s19171;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from student";
                con.Open();
                var dr = com.ExecuteReader();
                var list = new List<Student>();
                while (dr.Read())
                {
                    var st = new Student();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.BirthDate = DateTime.Parse(dr["BirthDate"].ToString());
                    list.Add(st);
                }
                return Ok(list);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent([FromRoute]int id)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s19171;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select E.semester, A.name, E.startdate from student S join enrollment E on S.idenrollment = E.idenrollment join Studies A on E.IdStudy=A.IdStudy where S.indexNumber=@id";
                com.Parameters.AddWithValue("id", "s"+id.ToString());
                con.Open();
                var dr = com.ExecuteReader();
                if(dr.Read())
                {
                    var st = new Enrollment();
                    st.Semester = Int32.Parse(dr["Semester"].ToString());
                    st.Study = dr["Name"].ToString();
                    st.StartDate = DateTime.Parse(dr["StartDate"].ToString());
                    return Ok(st);
                }
                else return NotFound("Student not found");
            }
        }

        [HttpPost]
        public IActionResult CreateStudent([FromBody]Student student)
        {
            student.IndexNumber=$"s{new Random().Next(1, 20000)}";
            _dbService.AddStudent(student);
            return Ok(student);
        }

        [HttpPut("{id:int}")]
        public IActionResult PutStudent(int id)
        {
            return Ok($"put {id}");
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteStudent(int id)
        {
            _dbService.DeleteStudent(id);
            return Ok($"deleted student with id: {id}");
        }

    }
}