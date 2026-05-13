using ChatIA.Domain.Enums;

namespace ChatIA.Domain.Entities;

public class ChatMessage
{
    public Guid Id { get; private set; }
    public Guid ChatId { get; private set; }
    public MessageRole Role { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ChatMessage(Guid chatId, MessageRole role, string content)
    {
        Id = Guid.NewGuid();
        ChatId = chatId;
        Role = role;
        Content = content;
        CreatedAt = DateTime.UtcNow;
    }
}