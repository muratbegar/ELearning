using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IskoopDemo.Shared.Infrastructure.File.Models;
using FileInfo = IskoopDemo.Shared.Infrastructure.File.Models.FileInfo;

namespace IskoopDemo.Shared.Application.Interfaces
{
    public interface IFileStorageService
    {
        // Upload operations
        Task<FileUploadResult> UploadAsync(Stream fileStream, string fileName, string containerName = null, CancellationToken cancellationToken = default);
        Task<FileUploadResult> UploadAsync(byte[] fileContent, string fileName, string containerName = null, CancellationToken cancellationToken = default);
        Task<FileUploadResult> UploadAsync(string filePath, string containerName = null, CancellationToken cancellationToken = default);

        // Download operations
        Task<Stream> DownloadAsync(string fileId, CancellationToken cancellationToken = default);
        Task<byte[]> DownloadBytesAsync(string fileId, CancellationToken cancellationToken = default);
        Task<string> DownloadToFileAsync(string fileId, string destinationPath, CancellationToken cancellationToken = default);


        // File information
        Task<FileInfo> GetFileInfoAsync(string fileId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string fileId, CancellationToken cancellationToken = default);
        Task<long> GetFileSizeAsync(string fileId, CancellationToken cancellationToken = default);

        // File operations
        Task<bool> DeleteAsync(string fileId, CancellationToken cancellationToken = default);
        Task<FileUploadResult> CopyAsync(string sourceFileId, string destinationFileName, string containerName = null, CancellationToken cancellationToken = default);
        Task<FileUploadResult> MoveAsync(string sourceFileId, string destinationFileName, string containerName = null, CancellationToken cancellationToken = default);

        // URL operations
        Task<string> GetDownloadUrlAsync(string fileId, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
        Task<string> GetUploadUrlAsync(string fileName, string containerName = null, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

        // Container operations
        Task<bool> CreateContainerAsync(string containerName, CancellationToken cancellationToken = default);
        Task<bool> DeleteContainerAsync(string containerName, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> ListContainersAsync(CancellationToken cancellationToken = default);

        // File listing 
        Task<IEnumerable<FileInfo>> ListFilesAsync(string containerName = null, string prefix = null, CancellationToken cancellationToken = default);
        Task<PagedFileList> ListFilesPagedAsync(string containerName = null, string prefix = null, int pageSize = 50, string continuationToken = null, CancellationToken cancellationToken = default);

        // Batch operations
        Task<IEnumerable<FileUploadResult>> UploadMultipleAsync(IEnumerable<FileUploadRequest> files, string containerName = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteMultipleAsync(IEnumerable<string> fileIds, CancellationToken cancellationToken = default);

        // Image operations (if applicable)
        Task<FileUploadResult> UploadImageAsync(Stream imageStream, string fileName, ImageResizeOptions resizeOptions = null, string containerName = null, CancellationToken cancellationToken = default);
        Task<Stream> GetThumbnailAsync(string fileId, int width, int height, CancellationToken cancellationToken = default);

    }
}
