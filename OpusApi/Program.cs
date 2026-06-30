using System.Reflection;
using Microsoft.OpenApi;
using OpusApi;
using OpusApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<InMemoryDbContext>();
builder.Services.AddDbContext<SqliteDbContext>();

builder.Services.AddEntityRepositories();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSignalR();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Opus API",
        Version = "v1",
        Description = "API приложения «Опус» — цифрового журнала оператора узла связи " +
                      "поисково-спасательного отряда ЛизаАлерт."
    });

    // Подключаем XML-документацию из сборки, чтобы описания методов и моделей
    // отображались в Swagger UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationsHub>("/notifications");

app.Run();