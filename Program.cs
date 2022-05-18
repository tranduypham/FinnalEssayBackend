using Backend.Models;
using Backend.Repository;
using Backend.Services.Certificate;
using Backend.Services.Encryption;
using Backend.Services.WIMServices;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IEncryptionServices, EncryptionServices>();
builder.Services.AddTransient<IBankingInfoRepo, BankingInfoRepo>();
builder.Services.AddTransient<ISessionKeyRepo, SessionKeysRepo>();
builder.Services.AddTransient<IWimServices, WimServices>();
builder.Services.AddTransient<ICertificateServices, CertificateServices>();

builder.Services.AddDbContext<SWPPDbContext>((options) => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithExposedHeaders("*"); // params string[];
                    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
