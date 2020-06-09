using AIS.Domain.ImageFiles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace AIS.Application.ImageFiles
{
    public class ImagePathFactory : IImagePathFactory
    {
        private readonly IFileSystem _fileSystem;

        public ImagePathFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public ImageFilePath CreateFilePathFromStringPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File is empty", nameof(filePath));

            if (!_fileSystem.File.Exists(filePath))
                throw new ArgumentException("File not found", nameof(filePath));

            var fileExtension = _fileSystem.Path.GetExtension(filePath).ToLower();

            var imageExtension = Enum.GetNames(typeof(ImageFileExtenstion)).FirstOrDefault(x => x == fileExtension[1..]);
            if (imageExtension is null)
                throw new ArgumentNullException($"File is not supported image", nameof(filePath));

            var imageFileExtenstion = (ImageFileExtenstion)Enum.Parse(typeof(ImageFileExtenstion), imageExtension, true);

            var imagePath = new ImageFilePath(filePath, imageFileExtenstion);
            return imagePath;
        }

        public ImageDirectoryPath CreateFolderPath(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                throw new ArgumentException("Folder path must not be empty", nameof(folderPath));

            if (!_fileSystem.Directory.Exists(folderPath))
                throw new ArgumentException("Path is not exist", nameof(folderPath));

            var imageFolderPath = new ImageDirectoryPath(folderPath);
            return imageFolderPath;
        }
    }
}
