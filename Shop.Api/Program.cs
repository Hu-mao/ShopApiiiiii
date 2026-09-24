using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Shop.Api.Interfaces;
using Shop.Api.Middlewares;
using Shop.Api.Services;
using Shop.Application.Interfaces;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Application.Mapping;
using Shop.Application.Services;
using Shop.Application.Validators.Category;
using Shop.Infrastructure.Configuration;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Helpers;
using Shop.Infrastructure.RabbitMQ;
using Shop.Infrastructure.Repositories;
using Shop.Infrastructure.Services;
using StackExchange.Redis;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
namespace Shop.Api;

//public static class MiddlewareExtensions
//{
//    public static IApplicationBuilder UseRequestTimer(this IApplicationBuilder builder)
//    {
//        return builder.UseMiddleware<RequestTimerMiddleware>();
//    }
//}
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<ShopDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));
        });

        builder.Services.AddAutoMapper(
            _ => { },
            typeof(CategoryProfile).Assembly,
            typeof(UserProfile).Assembly
        );


        var configuration = builder.Configuration;
        builder.Services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        // ================= JWT Settings =================
        builder.Services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
      ?? throw new Exception("JWT settings not configured.");
        // ================= CORS =================
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // ================= Swagger + JWT =================
        builder.Services.AddSwaggerGen(options =>
        {

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {

                Type = SecuritySchemeType.Http,

                Scheme = "bearer",

                BearerFormat = "JWT",

                Name = "Authorization",

                In = ParameterLocation.Header,

                Description = "Enter JWT token"
            });


            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {

                [new OpenApiSecuritySchemeReference("Bearer", document)] = []

            });

        });
        //======================Redis=====================
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>

        {

            var config = builder.Configuration.GetConnectionString("RedisServerConnection");

            return ConnectionMultiplexer.Connect(config);

        });
        //builder.Services.AddSwaggerGen();

        // Add services to the container.
        //DI container
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        //--------------SERVICES
        builder.Services.AddScoped<Interfaces.IProductService, Services.ProductService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IImageService, ImageService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IJWTService, JWTService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IAdminService, AdminService>();
        //--------------HELPERS
        builder.Services.AddSingleton<IHashHelper, HashHelper>();
        //--------------REPOSITORIES
        //--------------REPOSITORIES
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<IAuthRepository, AuthRepository>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<Interfaces.IProductService, Services.ProductService>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        //VALIDATORS
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<CategoryCreateValidator>();






        builder.Services.AddScoped<RabbitMQProducer>();


        builder.Services.AddMemoryCache();

        //builder.Services.AddScoped<ICachingService, MemoryCachingService>();
        builder.Services.AddScoped<ICachingService, RedisCachingService>();
        // ================= AUTHENTICATION (JWT + COOKIES + GOOGLE) =================
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })

     .AddJwtBearer(options =>

     {

         options.TokenValidationParameters = new TokenValidationParameters
         {

             ValidateIssuer = true,

             ValidateAudience = true,

             ValidateLifetime = true,

             ValidateIssuerSigningKey = true,


             ValidIssuer = jwtSettings.Issuer,

             ValidAudience = jwtSettings.Audience,


             IssuerSigningKey = new SymmetricSecurityKey(

                 Encoding.UTF8.GetBytes(jwtSettings.Key)

             ),


             ClockSkew = TimeSpan.Zero

         };

     })

     .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme).AddGoogle(options =>

     {

            options.ClientId = configuration["Authentication:Google:ClientId"]!;

            options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;

            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        });
        //===============Cors
        builder.Services.AddCors(options =>

        {

            options.AddPolicy("AllowAll", policy =>

            {

                policy.AllowAnyOrigin()

                      .AllowAnyMethod()

                      .AllowAnyHeader();

            });

        });
        //builder.Services.AddCors(options =>
        //{
        //    options.AddPolicy("ProductionPolicy", policy =>
        //    {
        //        policy.WithOrigins("https://example.com", "https://www.example.com")
        //              .WithMethods("GET", "POST", "PUT", "DELETE")
        //              .WithHeaders("Content-Type", "Authorization");
        //    });
        //});
        builder.Services.AddHostedService<RabbitMqReaderService>();
        builder.Services.AddSingleton<IQueueService, RabbitMqService>();
        builder.Services.AddAuthorization();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        //builder.Services.AddOpenApi();
        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider
                .GetRequiredService<ShopDbContext>();

            var hashHelper = scope.ServiceProvider
                .GetRequiredService<IHashHelper>();

            await AdminSeeder.SeedAsync(context, hashHelper);
        }
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors("AllowAll");
        //app.UseCors("ProductionPolicy");

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
        //    app.MapOpenApi();
        //}

        //app.UseHttpsRedirection();

        //app.UseAuthorization();
        app.UseAuthentication();

        app.UseAuthorization();
        app.UseMiddleware<RequestTimerMiddleware>();
        app.UseStaticFiles();
        app.MapControllers();



        app.Run();
    }
}
