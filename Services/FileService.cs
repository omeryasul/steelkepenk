using Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace Persistence.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly string[] _allowedDocumentExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder = "images")
        {
            ValidateImageFile(file);

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{folder}/{fileName}";
        }

        public async Task<string> UploadImageAsync(Stream stream, string fileName, string folder = "images")
        {
            if (!IsValidImageFile(fileName))
                throw new ArgumentException("Desteklenmeyen resim formatı.");

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await stream.CopyToAsync(fileStream);

            return $"/uploads/{folder}/{uniqueFileName}";
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder = "documents")
        {
            ValidateFile(file);

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{folder}/{fileName}";
        }

        public async Task<string> UploadFileAsync(Stream stream, string fileName, string folder = "documents")
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await stream.CopyToAsync(fileStream);

            return $"/uploads/{folder}/{uniqueFileName}";
        }

        public async Task<bool> DeleteFileAsync(string relativePath)
        {
            return await Task.FromResult(DeleteFile(relativePath));
        }

        public bool DeleteFile(string relativePath)
        {
            try
            {
                if (string.IsNullOrEmpty(relativePath))
                    return false;

                var fullPath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/'));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool FileExists(string relativePath)
        {
            try
            {
                if (string.IsNullOrEmpty(relativePath))
                    return false;

                var fullPath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/'));
                return File.Exists(fullPath);
            }
            catch
            {
                return false;
            }
        }

        public long GetFileSize(string relativePath)
        {
            try
            {
                if (string.IsNullOrEmpty(relativePath))
                    return 0;

                var fullPath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/'));
                var fileInfo = new FileInfo(fullPath);
                return fileInfo.Exists ? fileInfo.Length : 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<string> ResizeImageAsync(string imagePath, int width, int height)
        {
            // Basit implementasyon - production'da ImageSharp gibi bir library kullanın
            await Task.CompletedTask;
            return imagePath; // Şimdilik orijinal path'i döndürüyoruz
        }

        public bool IsValidImageFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return _allowedImageExtensions.Any(ext => ext == extension);
        }

        public bool IsValidFileSize(long fileSize, long maxSize = 5242880)
        {
            return fileSize > 0 && fileSize <= maxSize;
        }

        private void ValidateImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Dosya seçilmedi.");

            if (!IsValidFileSize(file.Length, MaxFileSize))
                throw new ArgumentException($"Dosya boyutu {MaxFileSize / (1024 * 1024)}MB'dan büyük olamaz.");

            if (!IsValidImageFile(file.FileName))
                throw new ArgumentException("Desteklenmeyen dosya formatı. Sadece resim dosyaları yüklenebilir.");
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Dosya seçilmedi.");

            if (!IsValidFileSize(file.Length, MaxFileSize))
                throw new ArgumentException($"Dosya boyutu {MaxFileSize / (1024 * 1024)}MB'dan büyük olamaz.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allAllowedExtensions = _allowedImageExtensions.Concat(_allowedDocumentExtensions);

            if (!allAllowedExtensions.Any(ext => ext == extension))
                throw new ArgumentException("Desteklenmeyen dosya formatı.");
        }
    }
}