using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SkillSwapHub.API.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// Add services to the container
// ============================================================

builder.Services.AddControllers();


// ============================================================
// CORS
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// ============================================================
// Swagger
// ============================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter your JWT token"
        });

    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});


// ============================================================
// JWT Authentication
// ============================================================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        )
                    )
            };

        // JWT error logging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine(
                    "JWT Authentication Failed: "
                    + context.Exception.Message
                );

                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                Console.WriteLine(
                    "JWT Challenge: "
                    + context.ErrorDescription
                );

                return Task.CompletedTask;
            }
        };
    });


// ============================================================
// Dependency Injection
// ============================================================

builder.Services.AddScoped<
    SkillSwapHub.API.Data.DbConnectionFactory>();

builder.Services.AddScoped<
    SkillSwapHub.API.Repositories.UserRepository>();

builder.Services.AddScoped<
    SkillSwapHub.API.Services.AuthService>();

builder.Services.AddScoped<
    SkillSwapHub.API.Repositories.SkillRepository>();

builder.Services.AddScoped<SwapRequestRepository>();

builder.Services.AddScoped<SessionRepository>();

    builder.Services.AddScoped<ReviewRepository>();

builder.Services.AddScoped<
    SkillSwapHub.API.Repositories.UserSkillRepository>();

// Match Repository
builder.Services.AddScoped<
    SkillSwapHub.API.Repositories.MatchRepository>();

builder.Services.AddScoped<
    SkillSwapHub.API.Repositories.SessionRequestRepository>();


// ============================================================
// Build application
// ============================================================

var app = builder.Build();


// ============================================================
// HTTP Request Pipeline
// ============================================================

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    _ = Task.Run(async () =>
    {
        await Task.Delay(1000);

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "https://localhost:7121/swagger",
            UseShellExecute = true
        });

        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = "https://localhost:7121/index.html",
            UseShellExecute = true
        });
    });
}

app.Run();