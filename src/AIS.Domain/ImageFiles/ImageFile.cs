using AIS.Domain.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AIS.Domain.ImageFiles
{
    public class ImageFile : IEntity
    {
        protected ImageFile(Md5Info md5, ImageFilePath filePath)
        {
            Md5 = md5;
            FilePath = filePath;
        }

        public int Id { get; private set; }
        public Md5Info Md5 { get; private set; }
        public ImageFilePath FilePath { get; private set; }

        public bool IsIdSet() => Id != 0;
        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException("Id for image file can be greater than 0");

            Id = id;
        }

        public void SetFilePath(ImageFilePath newImageFilePath)
        {
            FilePath = newImageFilePath;
        }

        public string GetPrintableImageFileInfo()
        {
            return $"Id: {Id}\r\nMd5: {Md5}\r\nFilePath: {FilePath}\r\n---------------------";
        }

        public static class Factory
        {
            public static ImageFile CreateFromFileImagePath(ImageFilePath imageFilePath)
            {
                var md5 = new Md5Info(imageFilePath);
                var newImageFile = new ImageFile(md5, imageFilePath);
                return newImageFile;
            }
        }
    }
}
