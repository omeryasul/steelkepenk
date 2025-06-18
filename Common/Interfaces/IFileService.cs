using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder = "images");
        Task<string> UploadImageAsync(Stream stream, string fileName, string folder = "images");
        Task<string> UploadFileAsync(IFormFile file, string folder = "documents");
        Task<string> UploadFileAsync(Stream stream, string fileName, string folder = "documents");
        Task<bool> DeleteFileAsync(string relativePath);
        bool DeleteFile(string relativePath);
        bool FileExists(string relativePath);
        long GetFileSize(string relativePath);
        Task<string> ResizeImageAsync(string imagePath, int width, int height);
        bool IsValidImageFile(string fileName);
        bool IsValidFileSize(long fileSize, long maxSize = 5242880); // 5MB default
    }
}
