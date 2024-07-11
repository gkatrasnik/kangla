using System.Text.Json;

namespace kangla_backend.Utilities

{
    public class JsonFileLoader
    {
        private readonly IWebHostEnvironment _env;

        public JsonFileLoader(IWebHostEnvironment env)
        {
            _env = env;
        }

        public T LoadJson<T>(string filePath)
        {
            using var reader = new StreamReader(filePath);
            var json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
