using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace website.Services;

public static class ThumbnailGenerator
{
    public static void GenerateThumbnails(string sourceDir, string thumbsDir, int maxWidth = 400)
    {
        Directory.CreateDirectory(thumbsDir);

        var files = Directory.GetFiles(sourceDir)
            .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                        f.EndsWith(".jpeg") || f.EndsWith(".png") || f.EndsWith(".webp"));

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            var thumbPath = Path.Combine(thumbsDir, fileName);

            if (File.Exists(thumbPath)) continue;

            using var image = Image.Load(file);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxWidth, 0)
            }));
            image.Save(thumbPath);
        }
    }
}
