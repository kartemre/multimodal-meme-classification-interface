using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using BLL.Services;
using BLL.Interfaces;
using BLL;
using DAL.Context;
using DAL.Data;

var builder = WebApplication.CreateBuilder(args);
var tokenKey = builder.Configuration["Token:SecurityKey"];
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

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
        ValidIssuer = builder.Configuration["Token:Issuer"],
        ValidAudience = builder.Configuration["Token:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddRateLimiter(options =>
{
    var rateLimiterConfig = builder.Configuration.GetSection("RateLimiter:PartnerApiRateLimit");
    var permitLimit = rateLimiterConfig.GetValue<int>("PermitLimit");
    var windowSeconds = rateLimiterConfig.GetValue<int>("WindowSeconds");

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("PartnerApiRateLimit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "global",
            factory: key => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = TimeSpan.FromSeconds(windowSeconds),
                QueueLimit = 0
            }
        ));

    options.OnRejected = async (context, _) =>
    {
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
    };
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
    //options.AddPolicy("AllowSpecificOrigins", policy =>
    //{
    //    policy.WithOrigins("https://your-frontend-domain.com")
    //          .AllowAnyHeader()
    //          .AllowAnyMethod();
    //});
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5298); // Listen on port 5298 for HTTP requests
});

//builder.Services.AddDbContext<MyContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPostService, PostService>();

// Add HttpClient
builder.Services.AddHttpClient<IPostService, PostService>();

builder.Services.AddBLLServices(connectionString);

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MyContext>();
        await DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
// app.UseMiddleware<ExceptionMiddleware>(); // Uncomment if ExceptionMiddleware exists

app.MapControllers();

app.Run();
