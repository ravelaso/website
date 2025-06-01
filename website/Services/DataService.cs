using System.Text.Json;

namespace website.Services;

public class DataService
{
    private readonly IWebHostEnvironment _env;
    private readonly string _dataPath;

    public DataService(IWebHostEnvironment env)
    {
        _env = env;
        _dataPath = Path.Combine(env.WebRootPath, "data");
        Directory.CreateDirectory(_dataPath);
    }

    public async Task<T> LoadDataAsync<T>(string fileName) where T : new()
    {
        var filePath = Path.Combine(_dataPath, fileName);

        if (!File.Exists(filePath))
        {
            return new T();
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<T>(json) ?? new T();
        }
        catch
        {
            return new T();
        }
    }

    public async Task SaveDataAsync<T>(string fileName, T data)
    {
        var filePath = Path.Combine(_dataPath, fileName);
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(filePath, json);
    }
}
