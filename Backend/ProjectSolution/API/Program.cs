using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using BLL.Services;
using BLL.Interfaces;
using BLL;

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
        ValidateIssuer = true,                                      //HNAG? S?TELER DENETLEMEYE ?Z?N VER?CEK
        ValidateAudience = true,                                    //?Z?N VER?LEN S?TELER DENETLEN?CEK
        ValidateLifetime = true,                                    //TOKEN?N YA?AM SÜRES?N? KONTROL EDER
        ValidateIssuerSigningKey = true,                            //TOKEN?N B?ZE A?T OLUP OLMADI?INI KONTROL EDER
        ValidIssuer = builder.Configuration["Token:Issuer"],          //TOKEN?N K?M?N TARAFINDAN OLU?TURULDU?UNU KONTROL EDER
        ValidAudience = builder.Configuration["Token:Audience"],      //TOKEN?N K?ME A?T OLDU?UNU KONTROL EDER
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)), //TOKENIN B?ZE A?T OLUP OLMADI?INI KONTROL EDER
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddRateLimiter(options =>
{
    var rateLimiterConfig = builder.Configuration.GetSection("RateLimiter:PartnerApiRateLimit");
    var permitLimit = rateLimiterConfig.GetValue<int>("PermitLimit");
    var windowSeconds = rateLimiterConfig.GetValue<int>("WindowSeconds");

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // IP bazl? rate limiting politikas?
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

    // Reddedilen istekler için özel mesaj
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

//builder.Services.AddDbContext<MyContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPostService, PostService>();

builder.Services.AddBLLServices(connectionString);

var app = builder.Build();

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
app.MapControllers();

app.Run();
