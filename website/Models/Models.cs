namespace website.Models;


public interface IProject
{
    string Id { get; set; }
    string Title { get; set; }
    string Description { get; set; }
    DateTime Date { get; set; }
}


public class MusicProject : IProject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public DateTime Date { get; set; } = DateTime.Now;
}

public class CodeProject : IProject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string GitHubLink { get; set; } = string.Empty;
    public string DemoLink { get; set; } = string.Empty;
    public List<string> Technologies { get; set; } = new();
    public DateTime Date { get; set; } = DateTime.Now;
}

public class MusicData
{
    public List<MusicProject> Projects { get; set; } = new();
}

public class CodeData
{
    public List<CodeProject> Projects { get; set; } = new();
}
