using System.Text;
using DotNetEnv;
using EnlightDenBackendAPI;
using EnlightDenBackendAPI.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
string envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");

// Check if the .env file exists
if (File.Exists(envFilePath))
{
    DotNetEnv.Env.Load();
}
else
{
    throw new Exception(
        ".env file not found. Please create one with the required environment variables."
    );
}

// Read API key and connection string from environment variables
string openAiApiKey =
    Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException(
        "OPENAI_API_KEY is not set in the environment variables."
    );

string connectionString =
    Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? throw new InvalidOperationException(
        "CONNECTION_STRING is not set in the environment variables."
    );

string jwtKey =
    Environment.GetEnvironmentVariable("JWT_KEY")
    ?? throw new InvalidOperationException("JWT_KEY is not set in the environment variables.");

string jwtIssuer =
    Environment.GetEnvironmentVariable("JWT_ISSUER")
    ?? throw new InvalidOperationException("JWT_ISSUER is not set in the environment variables.");

string jwtAudience =
    Environment.GetEnvironmentVariable("JWT_AUDIENCE")
    ?? throw new InvalidOperationException("JWT_AUDIENCE is not set in the environment variables.");

// Validate that JWT configuration is not missing
if (
    string.IsNullOrEmpty(jwtKey)
    || string.IsNullOrEmpty(jwtIssuer)
    || string.IsNullOrEmpty(jwtAudience)
)
{
    throw new Exception(
        "JWT configuration values are missing. Check your .env file or environment variables."
    );
}

// Configure services and database connection
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add CORS configuration to allow requests from your frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:3000") // Adjust to your frontend's URL
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

// Add Swagger with JWT Bearer configuration
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description =
                "Enter 'Bearer' [space] and then your valid token.\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
        }
    );

    c.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                new string[] { }
            },
        }
    );
});

// Configure Entity Framework and the ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// Configure Identity with default token providers
builder
    .Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT authentication
builder
    .Services.AddAuthentication(options =>
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
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });

// Add authorization services if needed
builder.Services.AddAuthorization();

var app = builder.Build();

// Run migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        await dbContext.Database.MigrateAsync(); // Apply any pending migrations
        Console.WriteLine("Migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
        throw; // Handle or log exceptions based on your needs
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // Enable Authentication Middleware
app.UseAuthorization(); // Enable Authorization Middleware
app.UseCors("AllowFrontend"); // Enable CORS for frontend
app.MapControllers(); // Map controller endpoints

app.Run();
