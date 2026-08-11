using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using kangla.DeviceSimulator;

const string DeviceAccessKey = "kangla-simulator-01";
const int DryValue = 3200;
const int WetValue = 1500;

var baseUrl = args.Length > 0 ? args[0].TrimEnd('/') : "https://localhost:7049";
var rawSoilMoisture = args.Length > 1 && int.TryParse(args[1], out var parsedHumidity) ? parsedHumidity : 2350;
var checkInSeconds = args.Length > 2 && int.TryParse(args[2], out var parsedInterval) ? parsedInterval : 60;

if (rawSoilMoisture is < 0 or > 4095 || checkInSeconds < 1)
{
    Console.Error.WriteLine("Raw soil moisture must be 0-4095 and check-in seconds must be at least 1.");
    return;
}

var soilMoisturePercentage = SoilMoistureCalibration.CalculatePercentage(rawSoilMoisture, DryValue, WetValue);

using var client = new HttpClient { BaseAddress = new Uri($"{baseUrl}/") };
client.DefaultRequestHeaders.Add("X-Device-Access-Key", DeviceAccessKey);

var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
Console.WriteLine($"Device simulator connected to {baseUrl} as access key '{DeviceAccessKey}'. Press Ctrl+C to stop.");

while (true)
{
    try
    {
        var response = await client.PostAsJsonAsync(
            "api/device/check-ins",
            new DeviceCheckInRequest(rawSoilMoisture, soilMoisturePercentage));
        if (!response.IsSuccessStatusCode)
        {
            var details = await response.Content.ReadAsStringAsync();
            Console.Error.WriteLine($"Check-in failed: {(int)response.StatusCode} ({response.ReasonPhrase}). {details}");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.Error.WriteLine("Ensure that the simulator device is in your inventory and attached to a plant before running it.");
            }

        }
        else
        {
            var checkIn = await response.Content.ReadFromJsonAsync<DeviceCheckInResponse>(jsonOptions)
                ?? throw new InvalidOperationException("The check-in response was empty.");

            Console.WriteLine($"{DateTimeOffset.Now:T} Check-in recorded; moisture: {soilMoisturePercentage}% (raw {rawSoilMoisture}).");
            if (checkIn.Command is not null)
            {
                await ExecuteWateringCommandAsync(client, checkIn.Command, jsonOptions);
            }
        }
    }
    catch (HttpRequestException exception)
    {
        Console.Error.WriteLine($"Check-in failed: {exception.Message}");
    }
    catch (InvalidOperationException exception)
    {
        Console.Error.WriteLine($"Simulator error: {exception.Message}");
    }

    await Task.Delay(TimeSpan.FromSeconds(checkInSeconds));
}

static async Task ExecuteWateringCommandAsync(HttpClient client, DeviceWateringCommand command, JsonSerializerOptions jsonOptions)
{
    Console.WriteLine($"Acknowledging watering command {command.Id}; simulated duration: {command.DurationSeconds}s.");
    var acknowledgement = await client.PostAsync($"api/device/watering-commands/{command.Id}/acknowledgements", null);
    acknowledgement.EnsureSuccessStatusCode();

    var startedAtUtc = DateTime.UtcNow;
    await Task.Delay(TimeSpan.FromSeconds(command.DurationSeconds));
    var finishedAtUtc = DateTime.UtcNow;

    var result = await client.PostAsJsonAsync(
        $"api/device/watering-commands/{command.Id}/results",
        new DeviceWateringCommandResult("Completed", startedAtUtc, finishedAtUtc));
    result.EnsureSuccessStatusCode();

    var completed = await result.Content.ReadFromJsonAsync<WateringCommandResponse>(jsonOptions);
    Console.WriteLine($"Watering command {command.Id} completed; API status: {completed?.Status ?? "unknown"}.");
}

internal sealed record DeviceCheckInRequest(int RawSoilMoisture, int SoilMoisturePercentage);

internal sealed record DeviceCheckInResponse(DeviceWateringCommand? Command);

internal sealed record DeviceWateringCommand(int Id, int DurationSeconds);

internal sealed record DeviceWateringCommandResult(
    string Outcome,
    [property: JsonConverter(typeof(UtcDateTimeConverter))] DateTime StartedAtUtc,
    [property: JsonConverter(typeof(UtcDateTimeConverter))] DateTime FinishedAtUtc);

internal sealed record WateringCommandResponse(string Status);

internal sealed class UtcDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetDateTime();

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToUniversalTime());
}
