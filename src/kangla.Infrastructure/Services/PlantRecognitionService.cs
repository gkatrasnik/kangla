using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<PlantRecognitionService> _logger;

        public PlantRecognitionService(
            IConfiguration configuration,
            ILogger<PlantRecognitionService> logger)
        {
            _logger = logger;

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
                // Plant identification is a bounded extraction task; hidden reasoning can consume
                // the complete output budget before the JSON response is produced.
#pragma warning disable OPENAI001
                ReasoningEffortLevel = ChatReasoningEffortLevel.None,
#pragma warning restore OPENAI001
                // GPT-5 models use output tokens for both reasoning and the visible response.
                // 800 tokens can therefore leave a partial JSON document to deserialize.
                MaxOutputTokenCount = 2000,
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "plant_recognition",
                    jsonSchema: PlantRecognitionSchema,
                    jsonSchemaFormatDescription: "A recognized plant and its care guidance.",
                    jsonSchemaIsStrict: true)
            };

            ChatCompletion chatCompletion = await _client.CompleteChatAsync(messages, options);

            if (chatCompletion.FinishReason == ChatFinishReason.Length)
            {
                _logger.LogWarning("Plant recognition response was truncated due to the output token limit.");
                return new PlantRecognitionResponse
                {
                    Error = "The recognition response was incomplete. Please try again."
                };
            }

            if (chatCompletion.FinishReason == ChatFinishReason.ContentFilter)
            {
                _logger.LogWarning("Plant recognition response was omitted by a content filter.");
                return new PlantRecognitionResponse
                {
                    Error = "The image could not be processed. Please try a different image."
                };
            }

            var jsonResponse = chatCompletion.Content.FirstOrDefault()?.Text;

            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                _logger.LogWarning("Plant recognition returned no text content. Finish reason: {FinishReason}", chatCompletion.FinishReason);
                return new PlantRecognitionResponse
                {
                    Error = "The recognition service returned no result. Please try again."
                };
            }

            try
            {
                return JsonSerializer.Deserialize<PlantRecognitionResponse>(jsonResponse) ?? new PlantRecognitionResponse
                {
                    Error = "The recognition service returned an empty result. Please try again."
                };
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Plant recognition returned invalid JSON. Finish reason: {FinishReason}", chatCompletion.FinishReason);
                return new PlantRecognitionResponse
                {
                    Error = "The recognition service returned an invalid result. Please try again."
                };
            }
        }
    }
}
