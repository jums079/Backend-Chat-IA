namespace ChatIA.Application.Interfaces;

public interface IAiService
{
    Task<string> SendMessageAsync(string message);
    Task<string> GenerateTitleAsync(string message);
}