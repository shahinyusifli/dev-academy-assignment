using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using DevAcademyAssigment.Models;



string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Messages") ?? "Data Source=Messages.db";

builder.Services.AddDbContext<MessageDb>(options => options.UseSqlite(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
   c.SwaggerDoc("v1", new OpenApiInfo { Title = "Messages API", Description = "Messages messages", Version = "v1" });
});

builder.Services.AddCors(options =>
{
  options.AddPolicy(name: MyAllowSpecificOrigins,
    builder =>
    {
         builder.WithOrigins("*");
    });
});







var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pizza API V1");
});

app.UseCors(MyAllowSpecificOrigins);

app.MapGet("/", () => "Hello World!");

app.Run();
