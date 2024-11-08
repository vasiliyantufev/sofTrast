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


// Ассоциативный массив (словарь) для IdTopic
Dictionary<string, int> topicDictionary = new Dictionary<string, int>
{
    { "Техподдержка", 0 },
    { "Продажи", 1 },
    { "Другое", 2 },
    { "Ещё один пункт", 3 }
};

// Определение маршрута POST для обратной связи
app.MapPost("/feedback", async (ApplicationDbContext dbContext, FeedbackModel feedback) =>
{
    if (string.IsNullOrWhiteSpace(feedback.Name) || 
        string.IsNullOrWhiteSpace(feedback.Email) || 
        string.IsNullOrWhiteSpace(feedback.Phone) || 
        string.IsNullOrWhiteSpace(feedback.Topic) || 
        string.IsNullOrWhiteSpace(feedback.Message))
    {
        return Results.BadRequest("Все поля обязательны для заполнения.");
    }

    int idTopicMsg;
    // Получение значения из словаря по ключу
    if (!topicDictionary.TryGetValue(feedback.Topic, out idTopicMsg))
    {
        return Results.BadRequest("Неверный идентификатор темы.");
    }
   //----------------------------------------------------------------- 
    // Запрос к базе данных с использованием LINQ
    var chkContact = await dbContext.Contacts
        .Where(c => c.Email == feedback.Email && c.Phone == feedback.Phone)
        .FirstOrDefaultAsync();

    int idContactMsg;       
    if (chkContact == null)
    {
        // Создание нового контакта
        var contact = new Contact
        {
            Name = feedback.Name,
            Email = feedback.Email,
            Phone = feedback.Phone,
        };
        // Добавление контакта в базу данных
        dbContext.Contacts.Add(contact);
        // Сохраняем изменения в базе данных и получаем ID нового контакта
        await dbContext.SaveChangesAsync();
        idContactMsg = contact.Id;
    } else {
        idContactMsg = chkContact.Id;
    }
   //----------------------------------------------------------------- 
    // Создание нового сообщения
    var Message = new Message
    {
        Id_contact = idContactMsg,
        Id_topic = idTopicMsg,
        Text = feedback.Message,
    };
    // Добавление сообщения в базу данных
    dbContext.Messages.Add(Message);
    await dbContext.SaveChangesAsync();
   //----------------------------------------------------------------- 

    // Возвращаем сообщение об успешной отправке
    return Results.Ok(new {
        feedback.Name,
        feedback.Email,
        feedback.Phone,
        feedback.Topic,
        feedback.Message});
})
.WithName("PostFeedback")
.WithOpenApi();

app.Run();

// Модель данных для обратной связи
public record FeedbackModel(string Name, string Email, string Phone, string Topic, string Message);
