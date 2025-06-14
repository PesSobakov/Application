
using Application.Server.Models.CoworkingDatabase;
using Application.Server.Models.DTOs;
using Application.Server.Models.Mapper;
using Application.Server.Models.Validation;
using Application.Server.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //CORS
            string CORSOpenPolicy = "OpenCORSPolicy";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                  name: CORSOpenPolicy,
                  builder =>
                  {
                      builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                  });
            });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthentication()
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromDays(10);
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Events.OnRedirectToAccessDenied =
                    options.Events.OnRedirectToLogin = c =>
                    {
                        c.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.FromResult<object>(null!);
                    };
                });
            builder.Services.AddDbContext<CoworkingContext>(
                options =>
                {
                    options.UseNpgsql(builder.Configuration["DATABASE_CONNECTION_STRING"], options2 =>
                    {
                        options2.CommandTimeout(300);
                    });
                });

            builder.Services.AddAutoMapper(typeof(CoworkingProfile).Assembly);
            builder.Services.AddValidatorsFromAssemblyContaining<StringDtoValidator>();
            builder.Services.AddTransient<ITimeProvider, Services.TimeProvider>();
            builder.Services.AddTransient<ICoworkingDatabaseService, CoworkingDatabaseService>();
            builder.Services.AddTransient<IGroqService, GroqService>();
            
            var app = builder.Build();

            //app.UseExceptionHandler();

            app.UseDefaultFiles();
            app.MapStaticAssets();

            app.UseCors(CORSOpenPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();

            //}

            app.UseHttpsRedirection();

            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
