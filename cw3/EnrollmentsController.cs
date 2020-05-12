using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using cw3.Models;
using cw3.Services;
using Microsoft.AspNetCore.Authorization;
using cw3.DTOs;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {

        private readonly IStudentsDbService _dbService;
        public EnrollmentsController(IStudentsDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult Enroll([FromBody] EnrollStudentRequest request)
        {
            if(request.FirstName==null||request.LastName==null||request.Studies==null||request.Birthdate==null||request.IndexNumber==null)
            {
                return BadRequest();
            }

            Enrollment enr = _dbService.EnrollStudent(request);
            if (enr != null)
            {
                return Ok();
            }
            else return BadRequest();
        }

        [HttpPost("promotions")]
        [Authorize(Roles = "employee")]
        public IActionResult PromoteStudents([FromBody]PromotionRequest request)
        {
            Enrollment result = _dbService.PromoteStudents(request.semester, request.studies);
            if (result == null)
            {
                return NotFound();
            }

            return Created("",result);
        }

    }
}
