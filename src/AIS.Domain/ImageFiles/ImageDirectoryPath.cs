using System;
using System.Collections.Generic;
using System.Text;

namespace AIS.Domain.ImageFiles
{
    public class ImageDirectoryPath
    {
        public readonly string Directory;

        public ImageDirectoryPath(string directory)
        {
            Directory = directory ?? throw new ArgumentNullException(nameof(directory));
        }
    }
}
