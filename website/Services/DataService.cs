using System.Text.Json;
using website.Models;

namespace website.Services;

public class DataService
{
    private readonly string _dataPath;

    public DataService(IWebHostEnvironment env)
    {
        _dataPath = Path.Combine(env.WebRootPath, "data");
        Directory.CreateDirectory(_dataPath);
    }

    public async Task<T> LoadDataAsync<T>(string fileName) where T : new()
    {
        var filePath = Path.Combine(_dataPath, fileName);

        if (!File.Exists(filePath))
        {
            Console.WriteLine("DataService.LoadDataAsync failed: No data file found.");
            return new T();
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<T>(json) ?? new T();
        }
        catch (Exception e)
        {
            Console.WriteLine("DataService.LoadDataAsync failed: {0}", e.Message);
            return new T();
        }
    }
    
    public async Task SaveDataAsync<T>(string fileName, T data)
    {
        try
        {
            var filePath = Path.Combine(_dataPath, fileName);
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(filePath, json);
        }
        catch (Exception e)
        {
            Console.WriteLine("DataService.SaveDataAsync failed: {0}", e.Message);
        }
    }

    // New generic method to add/update an item in a container
    public async Task AddOrUpdateItemAsync<T, TContainer>(
        T item,
        string fileName,
        Func<TContainer, List<T>> getItems)
        where T : IData
        where TContainer : new()
    {
        // Load current data container (for example CodeData or MusicData)
        var container = await LoadDataAsync<TContainer>(fileName);
        var list = getItems(container);
        var existing = list.FirstOrDefault(x => x.Id == item.Id);
        if (existing != null)
        {
            // Remove the existing item; alternatively you could update properties individually
            list.Remove(existing);
        }
        list.Add(item);
        await SaveDataAsync(fileName, container);
    }
}