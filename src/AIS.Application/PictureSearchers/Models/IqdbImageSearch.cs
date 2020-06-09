using System;

namespace AIS.Application.PictureSearchers.Models
{
    public class IqdbImageSearch : IIqdbImageSearchResult
    {
        public ImagePoolSite Site { get; private set; }
        public int Similarity { get; private set; }
        public Uri Uri { get; private set; }
        public Resolution Resolution { get; private set; }
        public EroRating EroRating { get; private set; }

        private IqdbImageSearch(string uri, int similarity, Resolution resolution, EroRating eroRating)
        {
            SetUri(uri);
            SetResolution(resolution);
            SetEroRating(eroRating);
            SetSimilarity(similarity);
        }

        private IqdbImageSearch SetSimilarity(int newSimilarity)
        {
            if (newSimilarity < 0)
                throw new ArgumentOutOfRangeException(nameof(newSimilarity), "Similarity can't be lesser than zero");

            Similarity = newSimilarity;
            return this;
        }

        private IqdbImageSearch SetUri(string newUri)
        {
            if (string.IsNullOrWhiteSpace(newUri))
            {
                throw new ArgumentException("Uri must not be empty", nameof(newUri));
            }

            if (newUri.StartsWith("//"))
                newUri = $"https:{newUri}";

            Uri uriResult;
            bool result = System.Uri.TryCreate(newUri, UriKind.RelativeOrAbsolute, out uriResult)
                && (uriResult.Scheme == System.Uri.UriSchemeHttp || uriResult.Scheme == System.Uri.UriSchemeHttps);
            if (!result)
                throw new ArgumentException($"String {newUri} is not Uri", nameof(newUri));
            Uri = uriResult;
            Site = ImagePoolSiteMethods.GetPoolSiteFromUri(Uri);
            return this;
        }

        private IqdbImageSearch SetResolution(Resolution newResolution)
        {
            if (newResolution.IsZero())
                throw new ArgumentOutOfRangeException("New resolution for image must be greater than zero");
            Resolution = newResolution;
            return this;
        }

        private IqdbImageSearch SetEroRating(EroRating newEroRating)
        {
            EroRating = newEroRating;
            return this;
        }

        public static class Factory
        {
            public static IqdbImageSearch Create(
                string uri,
                int similarity,
                Resolution resolution,
                EroRating eroRating) =>
                    new IqdbImageSearch(uri, similarity, resolution, eroRating);
        }
    }
}
