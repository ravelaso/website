using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Components.Forms;
using website.Models;
namespace website.Services;

public class ImageService
{
    private readonly string _galleryPath;
    private readonly string _thumbsPath;

    public ImageService(IWebHostEnvironment env)
    {
        _galleryPath = Path.Combine(env.WebRootPath, "images", "gallery");
        _thumbsPath = Path.Combine(env.WebRootPath, "images", "thumbs");

        // Ensure directories exist
        Directory.CreateDirectory(_galleryPath);
        Directory.CreateDirectory(_thumbsPath);
    }

    public Task<List<ImageData>> GetAllImagesAsync()
    {
        var images = new List<ImageData>();

        if (!Directory.Exists(_galleryPath))
            return Task.FromResult(images);

        var files = Directory.GetFiles(_galleryPath)
            .Where(IsValidImageFile)
            .OrderByDescending(File.GetCreationTime);

        images.AddRange(from file in files
        let fileName = Path.GetFileName(file)
        let thumbPath = Path.Combine(_thumbsPath, fileName)
        let fileInfo = new FileInfo(file)
        select new ImageData
        {
            Name = fileName,
            FullPath = file,
            ThumbPath = thumbPath,
            HasThumbnail = File.Exists(thumbPath),
            FileSize = fileInfo.Length,
            CreatedDate = fileInfo.CreationTime,
            ModifiedDate = fileInfo.LastWriteTime
        });

        return Task.FromResult(images);
    }

    public async Task<bool> UploadImageAsync(IBrowserFile file)
    {
        if (!IsValidImageFile(file.Name))
            return false;

        try
        {
            // Generate unique filename to prevent conflicts
            var fileName = $"{SanitizeFileName(file.Name)}";
            var filePath = Path.Combine(_galleryPath, fileName);

            // Save original image
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.OpenReadStream(maxAllowedSize: 100 * 1024 * 1024).CopyToAsync(stream); // 100MB limit
            }

            // Generate thumbnail
            await GenerateThumbnailAsync(filePath, fileName);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task<bool> DeleteImageAsync(string fileName)
    {
        try
        {
            var imagePath = Path.Combine(_galleryPath, fileName);
            var thumbPath = Path.Combine(_thumbsPath, fileName);

            // Delete original image
            if (File.Exists(imagePath))
                File.Delete(imagePath);

            // Delete thumbnail
            if (File.Exists(thumbPath))
                File.Delete(thumbPath);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task GenerateAllThumbnailsAsync()
    {
        var images = await GetAllImagesAsync();

        foreach (var image in images.Where(i => !i.HasThumbnail))
        {
            await GenerateThumbnailAsync(image.FullPath, image.Name);
        }
    }

    private async Task GenerateThumbnailAsync(string imagePath, string fileName, int maxWidth = 400)
    {
        try
        {
            var thumbPath = Path.Combine(_thumbsPath, fileName);

            if (File.Exists(thumbPath))
                return;

            using var image = await Image.LoadAsync(imagePath);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxWidth, 0)
            }));

            await image.SaveAsync(thumbPath);
        }
        catch
        {
            // Thumbnail generation failed, but don't throw
        }
    }

    private static bool IsValidImageFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension is ".jpg" or ".jpeg" or ".png" or ".webp" or ".gif";
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
    }
}
