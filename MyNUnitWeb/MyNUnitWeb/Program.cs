using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Models;
using MyNUnitWeb.Repositories;
using MyNUnitWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FileTestContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("FileTestsDb")));
builder.Services.AddScoped<IFileTestsRepository, FileTestsRepository>();
builder.Services.AddScoped<ITestsService, TestsService>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<FileTestContext>();
dbContext.Database.EnsureCreated();

app.UseRouting();
app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials()); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
