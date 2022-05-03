using Microsoft.EntityFrameworkCore;

namespace DevAcademyAssigment.Models 
{
  public class Message
  {
      public int MessageId { get; set; }
      public string? MessageContent { get; set; }
      public string? TopicName { get; set; }
      public DateTime? Date { get; set;  }


  }

  class MessageDb : DbContext
{
    public MessageDb(DbContextOptions options) : base(options) { }
    public DbSet<Message> Messages { get; set; }
}
}
