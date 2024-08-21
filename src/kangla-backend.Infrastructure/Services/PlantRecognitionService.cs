using Domain.Interfaces;

using Microsoft.Extensions.Configuration;
using Domain.Model;
using OpenAI.Chat;
using System.Text.Json;


namespace Infrastructure.Services
{
    public class PlantRecognitionService : IPlantRecognitionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public PlantRecognitionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI_ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("API Key for OpenAI is not configured.");
            }
        }


        public async Task<PlantRecognitionResult> RecognizePlantAsync(byte[] imageData)
        {
            ChatClient client = new("gpt-4o-mini", _apiKey);


            BinaryData imageBytes = BinaryData.FromBytes(imageData);

            //TODO use structured response
            List<ChatMessage> messages = [
                new SystemChatMessage("You are a plant recognition model. You are provided with a base64-encoded image of a plant. Recognize the plant and return a structured response with the following properties: CommonName, LatinName, WateringInstructions, WateringInterval (in days), AdditionalTips, Error. Each property should be a maximum of 5 sentences long. If recognition is successful, the error property should be empty. If there is no plant on the image or it cannot be recognized, return the structured response with empty values, but fill the error property explaining what went wrong."
),
                new UserChatMessage(
                ChatMessageContentPart.CreateImageMessageContentPart(imageBytes, "image/png") // image detail here - low?
                )
            ];

            ChatCompletion chatCompletion = client.CompleteChat(messages);


            var jsonResponse = chatCompletion.Content[0].Text;
            PlantRecognitionResult plantRecognitionResult;

            try
            {
                plantRecognitionResult = JsonSerializer.Deserialize<PlantRecognitionResult>(jsonResponse) ?? new PlantRecognitionResult();
            }
            catch (JsonException ex)
            {
                // Handle JSON parsing errors
                Console.WriteLine($"Error parsing JSON response: {ex.Message}");
                plantRecognitionResult = new PlantRecognitionResult
                {
                    Error = "Failed to parse the response."
                };
            }

            return plantRecognitionResult;
        }

    }
}
