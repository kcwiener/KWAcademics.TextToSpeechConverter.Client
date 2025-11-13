using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace KWAcademics.TextToSpeechConverter.Client.Services;

public class TextToSpeechApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _apiBaseUrl;

    public TextToSpeechApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        // Get API base URL from configuration
        _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7047";
    }

    public class ConversionResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public TimeSpan Duration { get; set; }
        public double Wpm { get; set; }
        public int WordCount { get; set; }
        public string? AudioDataUrl { get; set; }
        public byte[]? AudioData { get; set; }
    }

    public async Task<ConversionResult> ConvertTextToSpeechAsync(string inputText, int prosodyRate = 0)
    {
        try
        {
            // Validate word count client-side
            int wordCount = inputText.Split(new[] { ' ', '\n', '\r', '\t' },
              StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length;

            if (wordCount > 200)
            {
                return new ConversionResult
                {
                    Success = false,
                    Message = $"Text is too long. Maximum word count is 200, but got {wordCount} words."
                };
            }

            var request = new SynthesizeRequest
            {
                Text = inputText,
                ProsodyRate = prosodyRate
            };

            var requestUrl = $"{_apiBaseUrl}/api/speech/synthesize";
            var response = await _httpClient.PostAsJsonAsync(requestUrl, request);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<SynthesizeResponse>();

                if (apiResponse == null)
                {
                    return new ConversionResult
                    {
                        Success = false,
                        Message = "Failed to parse response from server"
                    };
                }

                // Decode base64 audio data
                byte[] audioData = Convert.FromBase64String(apiResponse.AudioDataBase64 ?? "");
                var audioDataUrl = $"data:audio/wav;base64,{apiResponse.AudioDataBase64}";

                return new ConversionResult
                {
                    Success = true,
                    Message = apiResponse.Message ?? "Conversion completed successfully",
                    Duration = TimeSpan.FromSeconds(apiResponse.DurationSeconds),
                    Wpm = apiResponse.Wpm,
                    WordCount = apiResponse.WordCount,
                    AudioDataUrl = audioDataUrl,
                    AudioData = audioData
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ConversionResult
                {
                    Success = false,
                    Message = $"Server error ({response.StatusCode}): {errorContent}"
                };
            }
        }
        catch (HttpRequestException httpEx)
        {
            return new ConversionResult
            {
                Success = false,
                Message = $"Network error: {httpEx.Message}. Make sure the API is running."
            };
        }
        catch (Exception ex)
        {
            return new ConversionResult
            {
                Success = false,
                Message = $"Error during conversion: {ex.Message}"
            };
        }
    }

    private record SynthesizeRequest
    {
        public string Text { get; init; } = string.Empty;
        public int ProsodyRate { get; init; }
    }

    private record SynthesizeResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }

        [JsonPropertyName("message")]
        public string? Message { get; init; }

        [JsonPropertyName("audioDataBase64")]
        public string? AudioDataBase64 { get; init; }

        [JsonPropertyName("durationSeconds")]
        public double DurationSeconds { get; init; }

        [JsonPropertyName("wpm")]
        public double Wpm { get; init; }

        [JsonPropertyName("wordCount")]
        public int WordCount { get; init; }
    }
}
