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






        public void DeleteImageAsync(string src)
        {
            if (string.IsNullOrWhiteSpace(src))
                return; 

            if (Uri.TryCreate(src, UriKind.Absolute, out var uri))
                src = uri.AbsolutePath; 

            var filePath = Path.Combine("wwwroot", src.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(filePath))
                File.Delete(filePath);
        }

    }
}
