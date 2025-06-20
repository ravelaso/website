namespace website.Models;


public interface IData
{
    public string Name { get; set; }
    public string? Id { get; set; }
}

public enum AboutType
{
    Music,
    Code
}

public class AboutEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public AboutType Type { get; set; }
}

public class ImageData: IData
{
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public string ThumbPath { get; set; } = string.Empty;
    public bool HasThumbnail { get; set; }
    public long FileSize { get; set; }
    public string? ImageFormat { get; set; }
    public byte[]? ImageBlob { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public string FormattedFileSize => FileSize switch
    {
        < 1024 => $"{FileSize} B",
        < 1024 * 1024 => $"{FileSize / 1024:F1} KB",
        < 1024 * 1024 * 1024 => $"{FileSize / (1024 * 1024):F1} MB",
        _ => $"{FileSize / (1024 * 1024 * 1024):F1} GB"
    };
}

public class MusicProject : IData
{
    public string? Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public DateTime Date { get; set; } = DateTime.Now;
}

public class CodeProject : IData
{
    public string? Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string GitHubLink { get; set; } = string.Empty;
    public string DemoLink { get; set; } = string.Empty;
    public List<string> Technologies { get; set; } = new();
    public DateTime Date { get; set; } = DateTime.Now;
}

public class AllowedUser
{
    public string Id { get; set; } = string.Empty; // GitHub user ID
    public string Username { get; set; } = string.Empty; // GitHub username
}