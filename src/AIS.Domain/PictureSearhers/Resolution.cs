using System;
using System.Data.Common;

namespace AIS.Domain.PictureSearhers
{
    public struct Resolution
    {
        private Resolution(int width, int height)
        {
            if (width < 0 || height < 0)
                throw new ArgumentException("Resolution cannot be less than zero");

            if((width == 0 || height == 0) && (width > 0 || height > 0))
                throw new ArgumentException("Resolution can't have only one zero parameter");
            Width = width;
            Height = height;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool IsZero() => Width == 0 && Height == 0;

        public static class Factory
        {
            public static Resolution CreateFromResolutionString(string resolutionString)
            {
                var resolutionArray = resolutionString.Split('×');
                Resolution resolution = new Resolution(int.Parse(resolutionArray[0]), int.Parse(resolutionArray[1]));
                return resolution;
            }

            public static Resolution GetZeroed() => new Resolution(0, 0);
        }
    }
}
