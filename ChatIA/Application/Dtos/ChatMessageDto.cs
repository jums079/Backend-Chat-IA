using ChatIA.Domain.Enums;

namespace ChatIA.Application.Dtos;

public class ChatMessageDto
{
   public Guid ChatiId { get; set; }
   public MessageRole Role { get; set; }
    public String Content { get; set; } = string.Empty;
}