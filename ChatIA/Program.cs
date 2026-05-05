using ChatIA.Application.Interfaces;
using ChatIA.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<AiService, GeminiService>();

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