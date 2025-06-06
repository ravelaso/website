using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Components.Forms;
using website.Models;
namespace website.Services;

public class ImageService
{
    private readonly string _galleryPath;
    private readonly string _thumbsPath;
    private readonly AppDbContext _context;

    public ImageService(IWebHostEnvironment env, AppDbContext context)
    {
        _galleryPath = Path.Combine(env.WebRootPath, "images", "gallery");
        _thumbsPath = Path.Combine(env.WebRootPath, "images", "thumbs");
        _context = context;
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
        let format = Path.GetExtension(file).ToLowerInvariant()
        let thumbPath = Path.Combine(_thumbsPath, fileName)
        let fileInfo = new FileInfo(file)
        select new ImageData
        {
            Id = fileName,
            Name = fileName,
            FullPath = file,
            ThumbPath = thumbPath,
            ImageFormat = format,
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
            var fileName = $"{SanitizeFileName(file.Name)}";
            var filePath = Path.Combine(_galleryPath, fileName);
            var format = Path.GetExtension(file.Name).ToLowerInvariant();
            await using var memoryStream = new MemoryStream();
            await file.OpenReadStream(maxAllowedSize: 100 * 1024 * 1024).CopyToAsync(memoryStream);

            // Save file to disk
            await using (var fs = new FileStream(filePath, FileMode.Create))
            {
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(fs);
            }

            await GenerateThumbnailAsync(filePath, fileName);

            var fileInfo = new FileInfo(filePath);

            var imageData = new ImageData
            {
                Id = fileName,
                Name = fileName,
                FullPath = filePath,
                ImageFormat = format,
                ThumbPath = Path.Combine(_thumbsPath, fileName),
                HasThumbnail = true,
                FileSize = fileInfo.Length,
                CreatedDate = fileInfo.CreationTime,
                ModifiedDate = fileInfo.LastWriteTime,
                ImageBlob = memoryStream.ToArray()
            };

            _context.Images.Add(imageData);
            await _context.SaveChangesAsync();

            return true;
        }
        catch(Exception e)
        {
            Console.WriteLine($"Failed to upload image: {e.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteImageAsync(ImageData image)
    {
        try
        {
            var imagePath = Path.Combine(_galleryPath, image.Name);
            var thumbPath = Path.Combine(_thumbsPath, image.Name);
            var imageDb = await _context.Images.FindAsync(image.Id);

            // Delete original image
            if (File.Exists(imagePath))
                File.Delete(imagePath);

            // Delete thumbnail
            if (File.Exists(thumbPath))
                File.Delete(thumbPath);

            _context.Images.Remove(imageDb!);

            return await Task.FromResult(true);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Failed to delete image: {ex.Message}");
            return await Task.FromResult(false);
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
