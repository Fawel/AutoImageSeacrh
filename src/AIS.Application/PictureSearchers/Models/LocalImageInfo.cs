using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;
using AIS.Domain.PictureSearhers;

namespace AIS.Application.PictureSearchers.Models
{
    public class LocalImageInfo
    {
        private LocalImageInfo(string name, string filePath, Resolution resolution)
        {
            Name = name;
            FilePath = filePath;
            Resolution = resolution;
        }

        public string Name { get; private set; }
        public string FilePath { get; private set; }
        public Resolution Resolution { get; private set; }

        public static class Factory
        {
            public static LocalImageInfo CreateFromFile(IFileSystem fileSystem, string filePath, Resolution resolution)
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    throw new ArgumentException("Path to file must be provided", nameof(filePath));
                }

                var fileExist = fileSystem.File.Exists(filePath);
                if (!fileExist)
                    throw new ArgumentException("Path to file is incorrect", nameof(filePath));

                var fileName = fileSystem.Path.GetFileNameWithoutExtension(filePath);

                return new LocalImageInfo(fileName, filePath, resolution);
            }
        }
    }
}
