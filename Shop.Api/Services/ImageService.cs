using Shop.Api.Interfaces;
using Shop.Api.Interfaces;
namespace Shop.Api.Services
{
    public class ImageService(IWebHostEnvironment environment) : IImageService
    {
        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            var folderPath = Path.Combine(environment.WebRootPath, folder);
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }
    }
}

