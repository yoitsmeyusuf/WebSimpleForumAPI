using ForumApi.Models;
using ForumApi.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Antiforgery;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)
            ),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.Configure<UserStoreDatabaseSettings>(
    builder.Configuration.GetSection("UserStoreDatabaseSettings")
);
builder.Services.Configure<SubjectStoreDatabaseSettings>(
    builder.Configuration.GetSection("SubjectStoreDatabaseSettings")
);
builder.Services.Configure<CommentDatabaseSettings>(
    builder.Configuration.GetSection("CommentDatabaseSettings")
);
builder.Services.Configure<EmailStorage>(
    builder.Configuration.GetSection("EmailStorage")
);

// builder.Services.AddScoped<TokenAuthenticationMiddleware>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MailServices>();
builder.Services.AddSingleton<UsersService>();
builder.Services.AddSingleton<SubjectService>();
builder.Services.AddSingleton<CommentService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// app.UseMiddleware<TokenAuthenticationMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllers();

app.Run();
