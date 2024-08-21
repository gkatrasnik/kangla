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


        public async Task<PlantRecognitionResponse> RecognizePlantAsync(byte[] imageData)
        {
            ChatClient client = new("gpt-4o-mini", _apiKey);


            BinaryData imageBytes = BinaryData.FromBytes(imageData);

            //TODO use structured response
            List<ChatMessage> messages = [
                new SystemChatMessage("You are a plant recognition model. You are provided with an image of a plant. Recognize the plant and return a structured response in JSON format with the following properties: CommonName, LatinName, Description, WateringInstructions, WateringInterval (recommended watering interval for this plant in days), AdditionalTips, Error. Each property should be a maximum of 5 sentences long. If you recognize the plant, the error property should be empty. If there is no plant on the image or you can not recognize the plant, the error property should contain an error message and all other properties should have null values."
),
                new UserChatMessage(
                ChatMessageContentPart.CreateImageMessageContentPart(imageBytes, "image/png") // image detail here - low?
                )
            ];

            ChatCompletionOptions options = new() {
                ResponseFormat = ChatResponseFormat.JsonObject

            };

            ChatCompletion chatCompletion = await client.CompleteChatAsync(messages, options);


            var jsonResponse = chatCompletion.Content[0].Text;
            PlantRecognitionResponse plantRecognitionResult;

            try
            {
                plantRecognitionResult = JsonSerializer.Deserialize<PlantRecognitionResponse>(jsonResponse) ?? new PlantRecognitionResponse();
            }
            catch (JsonException ex)
            {
                // Handle JSON parsing errors
                Console.WriteLine($"Error parsing JSON response: {ex.Message}");
                plantRecognitionResult = new PlantRecognitionResponse
                {
                    Error = "Failed to parse the response."
                };
            }

            return plantRecognitionResult;
        }

    }
}
