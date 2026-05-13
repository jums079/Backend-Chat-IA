namespace ChatIA.Application.Dtos;

public class ChatHistoryResponseDto
{
   public string Title { get; set; } = string.Empty;
   public List<ChatMessageDto> Messages { get; set; } = [];
}