using ChatIA.Application.Interfaces;
using ChatIA.Infrastructure.Data;
using ChatIA.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<AiService, GeminiService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Chat I.A API funcionando!");

app.MapPost("/ai/message", async (
        SendMessageRequest request,
        AiService aiService
    ) =>
    {
        var response = await aiService.SendMessageAsync(request.Message);

        return Results.Ok(new
        {
            answer = response
        });
    })
    .WithName("SendMessage")
    .WithOpenApi();

app.Run();

public record SendMessageRequest(string Message);