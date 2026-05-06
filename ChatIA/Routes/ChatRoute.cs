using ChatIA.Application.Dtos;
using ChatIA.Application.Interfaces;
using ChatIA.Domain.Entities;
using ChatIA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatIA.Routes;


public static class ChatRoutes
{
    public static void MapChatRoutes(this WebApplication app)
    {
        //POST - CRIA UM NOVO CHAT
        app.MapPost("/chats", async (
                CreateChatDto request,
                AppDbContext dbContext
            ) =>
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    return Results.BadRequest(new
                    {
                        message = "O título do chat é obrigatório."
                    });
                }

                var chat = new Chat(request.Title);

                dbContext.Chats.Add(chat);
                await dbContext.SaveChangesAsync();

                return Results.Created($"/chats/{chat.Id}", new
                {
                    chat.Id,
                    chat.Title,
                    chat.CreatedAt
                });
            })
            .WithName("CreateChat")
            .WithOpenApi();
        
        //GET DOS CHATS
        app.MapGet("/chats", async (AppDbContext dbContext) =>
            {
                var chats = await dbContext.Chats
                    .OrderByDescending(chat => chat.CreatedAt)
                    .Select(chat => new
                    {
                        chat.Id,
                        chat.Title,
                        chat.CreatedAt
                    })
                    .ToListAsync();

                return Results.Ok(chats);
            })
            .WithName("GetChats")
            .WithOpenApi();
        
        //POST - CRIA UMA NOVA MENSAGEM PARA 'TAL' CHAT
        app.MapPost("/chats/{chatId:guid}/messages", async (
                Guid chatId,
                SendChatMessageDto request,
                AppDbContext dbContext,
                AiService aiService
            ) =>
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return Results.BadRequest(new
                    {
                        message = "A mensagem é obrigatória."
                    });
                }

                var chatExists = await dbContext.Chats.AnyAsync(chat => chat.Id == chatId);

                if (!chatExists)
                {
                    return Results.NotFound(new
                    {
                        message = "Chat não encontrado."
                    });
                }

                var userMessage = new ChatMessage(
                    chatId,
                    "user",
                    request.Message
                );

                dbContext.ChatMessages.Add(userMessage);
                await dbContext.SaveChangesAsync();

                var aiResponse = await aiService.SendMessageAsync(request.Message);

                var assistantMessage = new ChatMessage(
                    chatId,
                    "assistant",
                    aiResponse
                );

                dbContext.ChatMessages.Add(assistantMessage);
                await dbContext.SaveChangesAsync();

                return Results.Ok(new
                {
                    chatId,
                    userMessage = request.Message,
                    assistantMessage = aiResponse
                });
            })
            .WithName("SendChatMessage")
            .WithOpenApi();
        
        //GET DO HISTORICO DE MENSAGENS DE 'TAL' CHAT
        app.MapGet("/chats/{chatId:guid}/messages", async (
                Guid chatId,
                AppDbContext dbContext
            ) =>
            {
                var chatExists = await dbContext.Chats.AnyAsync(chat => chat.Id == chatId);

                if (!chatExists)
                {
                    return Results.NotFound(new
                    {
                        message = "Chat não encontrado."
                    });
                }

                var messages = await dbContext.ChatMessages
                    .Where(message => message.ChatId == chatId)
                    .OrderBy(message => message.CreatedAt)
                    .Select(message => new
                    {
                        message.Id,
                        message.ChatId,
                        message.Role,
                        message.Content,
                        message.CreatedAt
                    })
                    .ToListAsync();

                return Results.Ok(messages);
            })
            .WithName("GetChatMessages")
            .WithOpenApi();
        
        
    }
}