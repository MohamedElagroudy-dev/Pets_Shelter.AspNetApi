using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class ImageManagementService : IImageManagementService
    {
        private readonly IFileProvider fileProvider;
        public ImageManagementService(IFileProvider fileProvider)
        {
            this.fileProvider = fileProvider;
        }

        public async Task<List<string>> AddImageAsync(IFormFileCollection files, string src)
        {
            List<string> SaveImageSrc = new List<string>();

            var imageDirectory = Path.Combine("wwwroot", "Images", src);

            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            foreach (var item in files)
            {
                if (item.Length > 0)
                {
                    var imageName = item.FileName;

                    var shortGuid = Guid.NewGuid().ToString("N").Substring(0, 8);

                    var uniqueImageName = $"{Path.GetFileNameWithoutExtension(imageName)}_{shortGuid}{Path.GetExtension(imageName)}";

                    var root = Path.Combine(imageDirectory, uniqueImageName);

                    using (FileStream stream = new FileStream(root, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }

                    var imageSrc = $"/Images/{src}/{Uri.EscapeDataString(uniqueImageName)}";
                    SaveImageSrc.Add(imageSrc);
                }
            }
            return SaveImageSrc;
        }
        public async Task<string?> AddSingleImageAsync(IFormFile file, string src)
        {
            if (file == null || file.Length == 0)
                return null;

            var imageDirectory = Path.Combine("wwwroot", "Images", src);

            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            var imageName = file.FileName;
            var shortGuid = Guid.NewGuid().ToString("N").Substring(0, 8);

            var uniqueImageName =
                $"{Path.GetFileNameWithoutExtension(imageName)}_{shortGuid}{Path.GetExtension(imageName)}";

            var fullPath = Path.Combine(imageDirectory, uniqueImageName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/Images/{src}/{Uri.EscapeDataString(uniqueImageName)}";
        }

        public void DeleteImageAsync(string src)
        {
            if (string.IsNullOrWhiteSpace(src))
                return;

            // If provided an absolute URL, get the path part
            if (Uri.TryCreate(src, UriKind.Absolute, out var uri))
                src = uri.AbsolutePath;

            // URL-decode any percent-encoded characters
            string unescaped = src;
            try { unescaped = Uri.UnescapeDataString(src); } catch { }

            // Build candidate file paths
            var relativePath = unescaped.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var directPath = Path.Combine("wwwroot", relativePath);

            if (File.Exists(directPath))
            {
                File.Delete(directPath);
                TryDeleteParentIfEmpty(directPath);
                return;
            }

            // Try using the original (maybe contains %XX) filename
            var relativePathEncoded = src.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var encodedPath = Path.Combine("wwwroot", relativePathEncoded);
            if (File.Exists(encodedPath))
            {
                File.Delete(encodedPath);
                TryDeleteParentIfEmpty(encodedPath);
                return;
            }

            // As a fallback, attempt to find the file by filename in the expected folder
            try
            {
                var folderPath = Path.GetDirectoryName(directPath);
                if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
                {
                    var filenameUnescaped = Path.GetFileName(relativePath);
                    var filenameEncoded = Path.GetFileName(relativePathEncoded);

                    var files = Directory.GetFiles(folderPath);
                    foreach (var f in files)
                    {
                        var fn = Path.GetFileName(f);
                        if (string.Equals(fn, filenameUnescaped, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(fn, filenameEncoded, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(Uri.EscapeDataString(fn), filenameEncoded, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(Uri.UnescapeDataString(fn), filenameUnescaped, StringComparison.OrdinalIgnoreCase))
                        {
                            File.Delete(f);
                            TryDeleteParentIfEmpty(f);
                            return;
                        }
                    }
                }
            }
            catch
            {
                // ignore fallback errors
            }
        }

        private void TryDeleteParentIfEmpty(string filePath)
        {
            try
            {
                var dir = Path.GetDirectoryName(filePath);
                if (string.IsNullOrEmpty(dir)) return;

                if (Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
                {
                    Directory.Delete(dir);
                }
            }
            catch
            {
                // ignore
            }
        }
    }
}
