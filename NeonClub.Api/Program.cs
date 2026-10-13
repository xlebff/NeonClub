using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NeonClub.Api.Data;
using NeonClub.Api.Services;
using System.Text;

namespace NeonClub.Api
{
    public partial class Program
    {
        private const string CorsPolicy = "FrontendCors";

        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // БД
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy, policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:5173",
                            "http://localhost:3000"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // JWT
            var jwt = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt["Issuer"],
                        ValidAudience = jwt["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();
            builder.Services.AddScoped<FileStorageService>();
            builder.Services.AddScoped<JwtService>();

            builder.Services.AddControllers();

            builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o =>
            {
                o.MultipartBodyLengthLimit = 10 * 1024 * 1024;  // 10 МБ на запрос
            });

            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.MigrateAsync();
                await DbSeeder.SeedRolesAsync(db);
                await DbSeeder.SeedAdminUserAsync(db);
                await DbSeeder.SeedStatusesAsync(db);
                await DbSeeder.SeedZonesAsync(db);
            }
            var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");
            Directory.CreateDirectory(uploadsPath);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
                RequestPath = "/uploads"
            });

            app.UseStaticFiles();

            app.MapGet("/ping", () => "pong");
            app.UseCors(CorsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            await app.RunAsync();
        }
    }
}