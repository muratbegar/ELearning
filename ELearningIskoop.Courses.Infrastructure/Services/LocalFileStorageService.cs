using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Infrastructure.Services
{
    // Local file storage implementation (Development için)
    // Production'da AWS S3 kullanılabilir
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _basePath;
        public LocalFileStorageService()
        {
            _basePath = Path.Combine(Environment.CurrentDirectory, "uploads");
            EnsureDirectoryExists(_basePath);
        }
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken)
        {
            //Güvenli dosya adı oluşturma
            var safeFileName = GenerateSafeFileName(fileName);
            var relativePath = Path.Combine("courses", DateTime.UtcNow.ToString("yyyy-MM"),safeFileName);
            var fullPath = Path.Combine(_basePath, relativePath);

            //Klasör  yoksa oluştur
            EnsureDirectoryExists(Path.GetDirectoryName(fullPath));

            //Dosyayı kaydet
            using (var fileStreamWrite = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await fileStream.CopyToAsync(fileStreamWrite, cancellationToken);
            }

            return relativePath.Replace(Path.DirectorySeparatorChar,'/');
        }

        public async Task<string> UploadFileAsync(byte[] fileBytes, string fileName, string contentType, CancellationToken cancellationToken)
        {
            using (var memoryStream = new MemoryStream(fileBytes))
            {
                return await UploadFileAsync(memoryStream, fileName, contentType, cancellationToken);
            }
        }

        public async Task<Stream> DownloadFileAsync(string filePath, CancellationToken cancellationToken)
        {
            var fullPath = Path.Combine(_basePath, filePath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("File not found", fullPath);
            }

            return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        }

        public Task DeleteFileAsync(string filePath, CancellationToken cancellationToken)
        {
            var fullPath = Path.Combine(_basePath, filePath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        public async Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken)
        {
            var fullPath = Path.Combine(_basePath, filePath.Replace('/', Path.DirectorySeparatorChar));
            return File.Exists(fullPath);
        }

        public async Task<long> GetFileSizeAsync(string filePath, CancellationToken cancellationToken)
        {
            var fullPath = Path.Combine(_basePath, filePath.Replace('/', Path.DirectorySeparatorChar));

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var fileInfo = new FileInfo(fullPath);
            return fileInfo.Length;
        }


        #region Yardımcı Methodlar

        private static void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        private static string GenerateSafeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var safeFileName = new StringBuilder(fileName);

            foreach (var c in invalidChars)
            {
                safeFileName.Replace(c.ToString(), "_");
            }

            var extension = Path.GetExtension(fileName);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(safeFileName.ToString());
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);

            return $"{nameWithoutExtension}_{uniqueId}{extension}";
        }

        #endregion
    }
}
