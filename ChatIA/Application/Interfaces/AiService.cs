namespace ChatIA.Application.Interfaces;

public interface AiService
{
    Task<string> SendMessageAsync(string message);
}