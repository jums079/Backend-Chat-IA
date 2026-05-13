using System.Text;
using System.Text.Json;
using ChatIA.Application.Interfaces;

namespace ChatIA.Infrastructure.Services;

public class GeminiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> SendMessageAsync(string message)
    {
        var apiKey = _configuration["Gemini:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("Chave do Gemini não configurada.");

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = message
                        }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";

        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao chamar Gemini: {error}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(responseJson);

        var answer = document
            .RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return answer ?? string.Empty;
    }



    public async Task<string> GenerateTitleAsync(string message)
    {
        var apiKey = _configuration["Gemini:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("Chave do Gemini não configurada.");

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = $"Aja como um gerador de títulos de chat. Resuma a mensagem do usuário a seguir em um título criativo de no máximo 3 palavras. Responda APENAS o título, sem aspas, sem pontos finais e sem explicações adicionais. Mensagem: {message}"
                        }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url =
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";

        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao chamar Gemini: {error}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(responseJson);

        var answer = document
            .RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return answer ?? string.Empty;
    }
    
    
    
}