using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавление CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Подключение к базе данных MSSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавление сервисов Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Применение политики CORS
app.UseCors("AllowSpecificOrigin"); 
app.UseHttpsRedirection();

// Определение маршрута POST для обратной связи
app.MapPost("/feedback", async (ApplicationDbContext dbContext, FeedbackModel feedback) =>
{
    if (string.IsNullOrWhiteSpace(feedback.Name) || 
        string.IsNullOrWhiteSpace(feedback.Email) || 
        string.IsNullOrWhiteSpace(feedback.Phone) || 
        string.IsNullOrWhiteSpace(feedback.Subject) || 
        string.IsNullOrWhiteSpace(feedback.Message))
    {
        return Results.BadRequest("Все поля обязательны для заполнения.");
    }

    // Создание нового отзыва
    var review = new Review
    {
        Name = feedback.Name,
        Email = feedback.Email,
        Phone = feedback.Phone,
        Subject = feedback.Subject,
        Message = feedback.Message
    };

    // Добавление отзыва в базу данных
    dbContext.Add(review);
    await dbContext.SaveChangesAsync();

    // Возвращаем сообщение об успешной отправке
    return Results.Ok(new { Message = "Спасибо за ваш отзыв, " + feedback.Name });
})
.WithName("PostFeedback")
.WithOpenApi();

app.Run();

// Модель данных для обратной связи
public record FeedbackModel(string Name, string Email, string Phone, string Subject, string Message);
