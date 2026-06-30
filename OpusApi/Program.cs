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


builder.Services.AddSwaggerGen();


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