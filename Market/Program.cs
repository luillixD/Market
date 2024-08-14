using Market.Data;
using Market.Data.Repositories;
using Market.Data.Repositories.Interfaces;
using Market.Mappings;
using Market.Middleware;
using Market.Services;
using Market.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
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

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    var logger = services.GetRequiredService<ILogger<Program>>();

                    logger.LogInformation("Checking database...");

                    if (context.Database.GetPendingMigrations().Any())
                    {
                        logger.LogInformation("Applying pending migrations...");
                        context.Database.Migrate();
                        logger.LogInformation("Migrations applied successfully.");
                    }
                    else
                    {
                        logger.LogInformation("Database is up to date. No migrations needed.");
                    }

                    // Aquí puedes añadir lógica para sembrar datos iniciales si es necesario
                    DatabaseSeeder.SeedData(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or initializing the database.");
                    throw; // Re-lanza la excepción para asegurar que la aplicación no se inicie con una base de datos en mal estado
                }
            }


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

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));

            services.Configure<Urls>(configuration.GetSection("URLs"));
            services.AddScoped<IS3Service, S3Service>();
            services.Configure<AWSOptions>(configuration.GetSection("AWS"));

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

            services.ConfigureJwtAuthentication(configuration);

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

    public class Urls
    {
        public string UI { get; set; }
        public string LN { get; set; }
    }

    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
    }

    public class AWSOptions
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
        public string Region { get; set; }
    }

}