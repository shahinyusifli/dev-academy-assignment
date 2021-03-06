using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using DevAcademyAssigment.Models;



string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("messages") ?? "Data Source=messages.db";

builder.Services.AddDbContext<MessageDb>(options => options.UseSqlite(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
   c.SwaggerDoc("v1", new OpenApiInfo { Title = "Message API", Description = "Message message", Version = "v1" });
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
c.SwaggerEndpoint("/swagger/v1/swagger.json", "Message API V1");
});

app.UseCors(MyAllowSpecificOrigins);




app.MapGet("/", () => "Hello World!");

//Presents the topics (topic name, message count, time of the last message)
app.MapGet("/messages", async(MessageDb db) => await db.Messages
.GroupBy(topic => topic.TopicName)
.Select(
  messageGroup => new {

     TopicName = messageGroup.Key, TotalMessages = messageGroup.Count(),
     TimeOfLastMessage = messageGroup.Max(f => f.Date) 

     }
).OrderBy(time => time.TimeOfLastMessage)
.ToListAsync());



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