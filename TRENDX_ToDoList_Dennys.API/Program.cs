using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TRENDX_ToDoList_Dennys.API.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("ToDoListCs");
builder.Services.AddDbContext<TarefaDbContext>(o => o.UseSqlServer(connectionString));
//builder.Services.AddDbContext<TarefaDbContext>(o => o.UseInMemoryDatabase("TarefaDb"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TRENDX - TESTE PRÁTICO | VAGA ANALISTA BACKEND .NET | API de Lista de Tarefas | Dennys Takao",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Email = "dennys.takao@gmail.com",
            Name = "Dennys Jun Takao",
            Url = new Uri("https://www.linkedin.com/in/dennystakao/")
        }
    });

    var xmlFile = "TRENDX_ToDoList_Dennys.API.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
