using System;

namespace AIS.Application.PictureSearchers.Models
{
    public class IqdbImage
    {
        public IqdbImage(Resolution resolution, EroRating eroRating)
        {
            SetResolution(resolution);
            SetEroRating(eroRating);
        }

        public Resolution Resolution { get; private set; }
        public EroRating EroRating { get; private set; }

        private IqdbImage SetResolution(Resolution newResolution)
        {
            if (newResolution.IsZero())
                throw new ArgumentOutOfRangeException("New resolution for image must be greater than zero");
            Resolution = newResolution;
            return this;
        }

        private IqdbImage SetEroRating(EroRating newEroRating)
        {
            EroRating = newEroRating;
            return this;
        }

        public static class Factory
        {
            public static IqdbImage Create(Resolution resolution, EroRating eroRating) =>
                new IqdbImage(resolution, eroRating);
        }
    }
}
