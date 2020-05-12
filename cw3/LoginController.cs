

using cw3.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace cw3.controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        public IConfiguration Configuration { get; set; }

        public LoginController(IConfiguration iconfig)
        {
            Configuration = iconfig;
        }

        [HttpPost("test")]
        [Authorize(Roles = "employee")]
        public IActionResult Test()
        {
            return Ok("ok");
        }

        [HttpPost]
        public IActionResult Login(LoginRequestDto request)
        {
            var index = String.Empty;
            var refreshToken = Guid.NewGuid();

            using (var con = new SqlConnection("Data Source=db-mssql; Initial Catalog=s19171;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "SELECT Password, Salt FROM Student WHERE IndexNumber = @index";
                com.Parameters.AddWithValue("index", request.Login);

                var salt = "";
                var password = "";

                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    return NotFound("Student not found");
                }
                else
                {
                    password = dr["Password"].ToString();
                    salt = dr["Salt"].ToString();
                }

                var hashPass = KeyDerivation.Pbkdf2(
                password: request.Password,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 1000,
                numBytesRequested: 256 / 8);
                var pass = Convert.ToBase64String(hashPass);
                Console.WriteLine(request.Password);
                Console.WriteLine(salt);
                Console.WriteLine(pass);
                if (!(pass==password))
                {
                    return Unauthorized("Bad password");
                }

            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "emp"),
                new Claim(ClaimTypes.Role, "employee")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
                );

            using (var con = new SqlConnection("Data Source=db-mssql; Initial Catalog=s19171;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "INSERT INTO Student (RefreshToken) VALUES (@refreshToken) WHERE IndexNumber = @index";
                com.Parameters.AddWithValue("refreshToken", refreshToken);
                com.Parameters.AddWithValue("index", request.Login);
            }

            Console.WriteLine(refreshToken);
            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken
            }); ;

        }

        [HttpPost("refresh-token/{token}")]
        public IActionResult RefreshToken(string refToken)
        {
            using (var con = new SqlConnection("Data Source=db-mssql; Initial Catalog=s19171;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                com.CommandText = "SELECT RefreshToken FROM Student WHERE RefreshToken = @refreshToken";
                com.Parameters.AddWithValue("refreshToken", refToken);

                var dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    return NotFound("Token not found");
                }
                else
                {
                    var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Name, "emp"),
                    new Claim(ClaimTypes.Role, "employee")
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: "Gakko",
                        audience: "Students",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(10),
                        signingCredentials: creds
                        );

                    return Ok(new
                    {
                        accessToken = new JwtSecurityTokenHandler().WriteToken(token)
                    });
                }
            }
        }
    }
}