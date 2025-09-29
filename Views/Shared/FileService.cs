namespace DigitalMagazineStore.Shared
    
{
    public interface IFileService
    {
        void DeleteFile(string fileName);
        Task<string> SaveFile(IFormFile file, string[] allowedExtensions);
    }

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFile(IFormFile file, string[] allowedExtensions)
        {
            var wwwPath = _environment.WebRootPath;
            var path = Path.Combine(wwwPath, "uploads"); // Changed "images" -> "uploads"
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException(
                    $"Only {string.Join(", ", allowedExtensions)} files are allowed."
                );
            }

            string fileName = $"{Guid.NewGuid()}{extension}";
            string fileNameWithPath = Path.Combine(path, fileName);

            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName; // return only the filename, not full path
        }

        public void DeleteFile(string fileName)
        {
            var wwwPath = _environment.WebRootPath;
            var filePath = Path.Combine(wwwPath, "uploads", fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                throw new FileNotFoundException($"File not found: {fileName}");
            }
        }
    }
}

