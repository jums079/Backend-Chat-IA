using ChatIA.Application.Dtos;
using ChatIA.Application.Interfaces;
using ChatIA.Domain.Entities;
using ChatIA.Domain.Enums;
using ChatIA.Infrastructure.Data;
using ChatIA.Shared.Errors;
using Microsoft.EntityFrameworkCore;

namespace ChatIA.Routes;


public static class ChatRoutes
{
    public static void MapChatRoutes(this WebApplication app)
    {
        //POST - CRIA UM NOVO CHAT
        app.MapPost("/chats", async (
                AppDbContext dbContext
            ) =>
            {
                var chat = new Chat("Nova Conversa"); 

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
        
        //POST - CRIA UMA NOVA MENSAGEM PARA UM CHAT ESPECIFICO
        app.MapPost("/chats/{chatId:guid}/messages", async (
                Guid chatId,
                SendChatMessageDto request,
                AppDbContext dbContext,
                IAiService aiService
            ) =>
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return ApiError.BadRequest("A mensagem é obrigatória");
                }

                var chat = await dbContext.Chats.FirstOrDefaultAsync(chat => chat.Id == chatId);

                if (chat == null)
                {
                    return ApiError.NotFound("Chat não encontrado.");
                }

                if (chat.Title == "Nova Conversa" || string.IsNullOrWhiteSpace(chat.Title))
                {
                    try 
                    {
                        var generatedTitle = await aiService.GenerateTitleAsync(request.Message);
                        chat.Title = generatedTitle;
                    }
                    catch 
                    {
                      
                    }
                }

                var userMessage = new ChatMessage(
                    chatId,
                    MessageRole.User,
                    request.Message
                );

                dbContext.ChatMessages.Add(userMessage);
                await dbContext.SaveChangesAsync();

                string aiResponse;

                try
                {
                    aiResponse = await aiService.SendMessageAsync(request.Message);
                }
                catch
                {
                    return ApiError.ExternalServiceError(
                        "Não foi possível obter resposta da IA no momento. Tente novamente."
                    );
                }
                

                var assistantMessage = new ChatMessage(
                    chatId,
                    MessageRole.Assistant,
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
        
        //GET DO HISTORICO DE MENSAGENS DE UM CHAT ESPECIFICO
        app.MapGet("/chats/{chatId:guid}/messages", async (
                Guid chatId,
                AppDbContext dbContext
            ) =>
            {
                var chat = await dbContext.Chats.FirstOrDefaultAsync(chat => chat.Id == chatId);

                if (chat == null)
                {
                    return ApiError.NotFound("Chat não encontrado.");
                }

                var messages = await dbContext.ChatMessages
                    .Where(message => message.ChatId == chatId)
                    .OrderBy(message => message.CreatedAt)
                    .Select(message => new ChatMessageDto
                    {
                        Content = message.Content,
                        Role = message.Role,
                        ChatiId = message.ChatId // Mantendo o seu nome 'ChatiId'
                    }).ToListAsync();

                return Results.Ok(new ChatHistoryResponseDto
                {
                    Title = chat.Title,
                    Messages = messages
                });
            })
            .WithName("GetChatMessages")
            .WithOpenApi();
        
        
    }
}