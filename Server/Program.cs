using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TestAuth.Server.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<TestAuthContext>(
    options => options.UseSqlite("filename=Data/Database/testauth.db")
);

builder.Services.AddSwaggerGen(
    // options => {
    //     options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    //     {
    //         Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
    //         In = ParameterLocation.Header,
    //         Name = "Authorization",
    //         Type = SecuritySchemeType.ApiKey
    //     });
    //     options.OperationFilter<SecurityRequirementsOperationFilter>();
    // }
);

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
//     options => {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
//                 builder.Configuration.GetSection("Jwt:Key").Value)),
//             ValidateIssuer = false,
//             ValidateAudience = false,
//         };
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
