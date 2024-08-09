using Market.Data;
using Market.Data.Repositories;
using Market.Data.Repositories.Interfaces;
using Market.Mappings;
using Market.Services;
using Market.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Market
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            ConfigureApp(app);

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Configuración básica
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Configuración de la base de datos
            var connectionString = configuration.GetConnectionString("DataBase");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Registro de servicios y repositorios
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();

            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<ISubcategoryService, SubcategoryService>();
            services.AddScoped<ISubcategoryRepository, SubcategoryRepository>();

            services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));


            // Configuración de AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Configuración de Swagger
            services.AddSwaggerGen();

            // Configuración de CORS
            var UrlUI = configuration.GetSection("URLs")["UI"];
            services.AddCors(options =>
            {
                options.AddPolicy("OnlyOurURLs", policy =>
                {
                    policy.WithOrigins(UrlUI)
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Configuración de autenticación JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
                });

            // TODO: Considera añadir Health Checks
            // services.AddHealthChecks();
        }

        private static void ConfigureApp(WebApplication app)
        {
            // Middleware de manejo de errores (debe ser el primero)
            app.UseErrorHandler();

            // Middleware de seguridad
            app.UseHttpsRedirection();

            // Configuración de CORS (debe ir antes de la autorización)
            app.UseCors("OnlyOurURLs");

            // Middleware de autenticación y autorización
            app.UseAuthentication();
            app.UseAuthorization();

            // Configuración de Swagger (solo en desarrollo)
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Mapeo de controladores
            app.MapControllers();

            // Asegurar que la base de datos está creada y las migraciones aplicadas
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();
            }

            // TODO: Considera añadir el endpoint de Health Checks
            // app.MapHealthChecks("/health");
        }
    }

    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
    }

}