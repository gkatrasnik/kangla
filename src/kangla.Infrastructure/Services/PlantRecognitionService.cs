using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.Text.Json;
using kangla.Domain.Interfaces;
using kangla.Domain.Model;

namespace kangla.Infrastructure.Services
{
    public class PlantRecognitionService : IPlantRecognitionService
    {
        private static readonly BinaryData PlantRecognitionSchema = BinaryData.FromString("""
            {
              "type": "object",
              "properties": {
                "CommonName": { "type": ["string", "null"] },
                "LatinName": { "type": ["string", "null"] },
                "Description": { "type": ["string", "null"] },
                "WateringInstructions": { "type": ["string", "null"] },
                "WateringInterval": { "type": ["integer", "null"] },
                "AdditionalTips": { "type": ["string", "null"] },
                "Error": { "type": "string" }
              },
              "required": [
                "CommonName",
                "LatinName",
                "Description",
                "WateringInstructions",
                "WateringInterval",
                "AdditionalTips",
                "Error"
              ],
              "additionalProperties": false
            }
            """);

        private readonly ChatClient _client;

        public PlantRecognitionService(IConfiguration configuration)
        {
            var apiKey = configuration["OpenAI_ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("API Key for OpenAI is not configured.");
            }

            var model = configuration["OpenAI:PlantRecognitionModel"] ?? "gpt-5-nano";
            _client = new ChatClient(model, apiKey);
        }

        public async Task<PlantRecognitionResponse> RecognizePlantAsync(byte[] imageData)
        {
            BinaryData imageBytes = BinaryData.FromBytes(imageData);

            List<ChatMessage> messages = [
                new SystemChatMessage("You are a plant-recognition service. Identify the most likely plant in the image. Keep each text value to 300 characters or fewer. WateringInterval is the recommended watering interval in days. If the plant cannot be recognized or the image does not contain a plant, set Error to a concise explanation and every other property to null. Otherwise, set Error to an empty string."),
                new UserChatMessage(
                    ChatMessageContentPart.CreateImagePart(imageBytes, "image/jpeg")
                )
            ];

            ChatCompletionOptions options = new()
            {
                MaxOutputTokenCount = 800,
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "plant_recognition",
                    jsonSchema: PlantRecognitionSchema,
                    jsonSchemaFormatDescription: "A recognized plant and its care guidance.",
                    jsonSchemaIsStrict: true)
            };

            ChatCompletion chatCompletion = await _client.CompleteChatAsync(messages, options);
            var jsonResponse = chatCompletion.Content[0].Text;
            PlantRecognitionResponse plantRecognitionResult;

            try
            {
                plantRecognitionResult = JsonSerializer.Deserialize<PlantRecognitionResponse>(jsonResponse) ?? new PlantRecognitionResponse();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing OpenAI response: {ex.Message}");
                plantRecognitionResult = new PlantRecognitionResponse
                {
                    Error = "Failed to parse the recognition response."
                };
            }

            return plantRecognitionResult;
        }
    }
}
