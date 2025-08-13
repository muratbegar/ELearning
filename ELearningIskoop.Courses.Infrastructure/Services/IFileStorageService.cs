using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELearningIskoop.Courses.Infrastructure.Services
{
    // Dosya depolama servisi interface'i
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream,string fileName,string contentType,CancellationToken cancellationToken);
        Task<string> UploadFileAsync(byte[] fileBytes, string fileName, string contentType, CancellationToken cancellationToken);
        Task<Stream> DownloadFileAsync(string filePath, CancellationToken cancellationToken);
        Task DeleteFileAsync(string filePath, CancellationToken cancellationToken);
        Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken);
        Task<long> GetFileSizeAsync(string filePath, CancellationToken cancellationToken);
    }
}
