using AIS.Domain.ImageFiles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AIS.Application.ImageFiles
{
    public interface IImagePathFactory
    {
        ImageFilePath CreateFilePathFromStringPath(string filePath);
        ImageDirectoryPath CreateFolderPath(string folderPath);
    }
}
