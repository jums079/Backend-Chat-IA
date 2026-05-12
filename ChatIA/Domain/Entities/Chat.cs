namespace ChatIA.Domain.Entities;

public class Chat
{
    public Guid Id { get; private set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; private set; }

    public List<ChatMessage> Messages { get; private set; } = [];

    public Chat(string title)
    {
        Id = Guid.NewGuid();
        Title = title;
        CreatedAt = DateTime.UtcNow;
    }
}