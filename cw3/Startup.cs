using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using System.IO;

public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IStudentsDbService, SqlServerDbService>();
            services.AddTransient<IDbService, SqlServerDbService>();
            services.AddAuthentication("AuthenticationBasic").AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("AuthenticationBasic",null);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = "Gakko",
                    ValidAudience = "Students",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]))
                };
            }); ;
        IMvcBuilder mvcBuilder = services.AddControllers().AddXmlSerializerFormatters();
           
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbService dbService)
        {
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseMiddleware<LoggingMiddleware>();

            app.Use(async (context, c) =>
            {
                if (context.Request.Headers.ContainsKey("Index"))
                {
                    var index = context.Request.Headers["Index"].ToString();
                    var bodyStream = string.Empty;

                    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        bodyStream = await reader.ReadToEndAsync();
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Nie podano indeksu naglowku");
                    return;
                }


            });
            app.UseMiddleware<CustomMiddleware>();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

