using System;
using AIS.Domain.PictureSearhers;

namespace AIS.Application.PictureSearchers.Models
{
    public class IqdbImageSearch : IIqdbImageSearchResult
    {
        public ImagePoolSite Site { get; private set; }
        public Similarity Similarity { get; private set; }
        public Uri Uri { get; private set; }
        public Resolution Resolution { get; private set; }
        public EroRating EroRating { get; private set; }

        private IqdbImageSearch(string uri, Similarity similarity, string resolution, string eroRating)
        {
            SetUri(uri);

            var parsedResolution = Resolution.Factory.CreateFromResolutionString(resolution);
            SetResolution(parsedResolution);

            var parsedEroRating = EroRatingMethods.GetRatingFromString(eroRating);
            SetEroRating(parsedEroRating);

            SetSimilarity(similarity);
        }

        private IqdbImageSearch SetSimilarity(Similarity newSimilarity)
        {
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

            bool result = System.Uri.TryCreate(newUri, UriKind.RelativeOrAbsolute, out var uriResult)
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
                string resolution,
                string eroRating) =>
                    new IqdbImageSearch(uri, similarity, resolution, eroRating);
        }
    }
}
