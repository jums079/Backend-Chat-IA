using ChatIA.Application.Interfaces;

namespace ChatIA.Routes;

public static class AiRoutes
{
    public static void MapAiRoutes(this WebApplication app)
    {
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
    }
}

public record SendMessageRequest(string Message);