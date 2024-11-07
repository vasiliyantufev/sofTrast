using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    public DbSet<Review> Reviews { get; set; }  // Таблица отзывов
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
