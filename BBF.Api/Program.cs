using System.Text;
using BBF.Api.Services;
using BBF.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<ITokenServiceClient, TokenServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:TokenService"]);
});

builder.Services.AddHttpClient<IUserDataServiceClient, UserDataServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:UserDataService"]);
});

builder.Services.AddHttpClient<IUserPhotoServiceClient, UserPhotoServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:UserPhotoService"]);
});

builder.Services.AddScoped<IUserProfileService, UserProfileService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var jwtSecret = builder.Configuration["JwtSettings:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JWT Secret no está configurado");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
