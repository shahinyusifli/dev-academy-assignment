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


app.MapGet("/messages", async(MessageDb db) => await db.Messages.ToListAsync());



app.MapPost("/create", async(MessageDb db, Message message) => {
await db.Messages.AddAsync(message);
await db.SaveChangesAsync();
return Results.Created($"/message/{message.MessageId}", message);
});

app.MapPut("/message/{messageId}", async (MessageDb db, Message updateMessage, int messageId) =>
{
var messageItem = await db.Messages.FindAsync(messageId);
if (messageItem is null) return Results.NotFound();
messageItem.TopicName = updateMessage.TopicName;
messageItem.TopicName = updateMessage.TopicName;
await db.SaveChangesAsync();
return Results.NoContent();
});


app.MapDelete("/message/{messageid}", async (MessageDb db, int messageid) =>
{
var todo = await db.Messages.FindAsync(messageid);
if (todo is null)
{
return Results.NotFound();
}
db.Messages.Remove(todo);
await db.SaveChangesAsync();
return Results.Ok();
 });
app.Run();