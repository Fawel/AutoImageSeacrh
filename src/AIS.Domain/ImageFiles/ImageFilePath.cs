using System.Runtime.CompilerServices;

namespace AIS.Domain.ImageFiles
{
    public class ImageFilePath
    {
        private readonly string _filePath;
        private readonly ImageFileExtenstion _imageFileExtenstion;

        public ImageFilePath(string filePath, ImageFileExtenstion imageFileExtenstion)
        {
            _filePath = filePath;
            _imageFileExtenstion = imageFileExtenstion;
        }

        public string GetValue() => _filePath;
        public ImageFileExtenstion GetImageFileExtenstion() => _imageFileExtenstion;

        public override string ToString()
        {
            return _filePath;
        }

        public static implicit operator string(ImageFilePath imageFilePath) => imageFilePath.GetValue();

    }
}
