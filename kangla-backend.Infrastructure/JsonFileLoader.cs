using System.Text.Json;

namespace Infrastructure

{
    public class JsonFileLoader
    {
        public T LoadJson<T>(string filePath)
        {
            using var reader = new StreamReader(filePath);
            var json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
