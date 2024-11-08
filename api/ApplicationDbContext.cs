using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    public DbSet<Review> Reviews { get; set; }      // Таблица отзывов
    public DbSet<Contact> Contacts { get; set; }    // Таблица контактов
    public DbSet<Message> Messages { get; set; }    // Таблица сообщений
}

public class Review
{
    public int Id { get; set; } 
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Subject { get; set; }
    public string? Phone { get; set; }
    public string? Message { get; set; }
}

public class Contact
{
   public int Id { get; set; } 
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
public class Message
{
    public int Id { get; set; } 
    public int Id_contact { get; set; }
    public int Id_topic { get; set; }
    public string? Text { get; set; }
}