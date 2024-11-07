using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Добавьте сервисы в контейнер
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Настройте конвейер HTTP-запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Определение маршрута POST для обратной связи
app.MapPost("/feedback", (FeedbackModel feedback) =>
{
    // Выполните здесь валидацию, если необходимо
    if (string.IsNullOrWhiteSpace(feedback.Name) ||
        string.IsNullOrWhiteSpace(feedback.Email) ||
        string.IsNullOrWhiteSpace(feedback.PhoneNumber) ||
        string.IsNullOrWhiteSpace(feedback.Message))
    {
        return Results.BadRequest("Все поля обязательны для заполнения.");
    }

    // Возвращаем сообщение об успешной отправке
    return Results.Ok(new { Message = "Спасибо за ваш отзыв!" });
})
.WithName("PostFeedback")
.WithOpenApi();

// Остальной код
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 10).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-55, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Модель данных для обратной связи
record FeedbackModel(string Name, string Email, string PhoneNumber, string Message);