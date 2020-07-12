using AIS.Domain.PictureSearhers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AIS.Application.PictureSearchers.Models
{
    public class IqdbSearchFileRequest : ISearchRequest
    {
        public IqdbSearchFileRequest(LocalImageInfo localImage, bool isCompleted, IqdbSearchConfiguration configuration)
        {
            LocalImage = localImage;
            IsCompleted = isCompleted;
            Configuration = configuration;
        }

        private LocalImageInfo _localImage;
        public LocalImageInfo LocalImage
        {
            get => _localImage;
            private set
            {
                if (value is null)
                    throw new ArgumentNullException("Путь к файлу не должен быть пусть", nameof(value));

                _localImage = value;
            }
        }
        public bool IsCompleted { get; private set; }
        public IqdbSearchConfiguration Configuration { get; private set; }

        public static IqdbSearchConfiguration GetDefaultConfiguration()
            => IqdbSearchConfiguration.Factory.CreateEmpty()
                .AddAllowedRatings(EroRatingMethods.GetAll())
                .AddSitesToSearch(ImageSite.Danbooru)
                .SetMinimumResolution(Resolution.Factory.GetZeroed())
                .SetMinSimularity(90);

        public static class Factory
        {
            public static IqdbSearchFileRequest CreateWithDefaultConfiguration(LocalImageInfo filePath)
                => new IqdbSearchFileRequest(filePath, false, GetDefaultConfiguration());

            public static IqdbSearchFileRequest Create(
                LocalImageInfo filePath,
                IqdbSearchConfiguration configuration)
                => new IqdbSearchFileRequest(filePath, false, configuration);

            public static IqdbSearchFileRequest CreateCompleted(
                LocalImageInfo filePath,
                IqdbSearchConfiguration configuration)
                => new IqdbSearchFileRequest(filePath, true, configuration);
        }
    }

    public class IqdbSearchConfiguration
    {
        private IqdbSearchConfiguration()
        {
        }

        public Similarity MinSimilarity { get; private set; }

        private HashSet<EroRating> _eroRatings { get; set; } = new HashSet<EroRating>();
        public EroRating[] AllowedRatings => _eroRatings.ToArray();
        public Resolution MinResolution { get; private set; }
        private HashSet<ImageSite> _sitesToSearch { get; set; } = new HashSet<ImageSite>();
        public ImageSite[] SitesToSearch => _sitesToSearch.ToArray();

        #region Методы для замены значений
        public IqdbSearchConfiguration SetMinSimularity(Similarity newValue)
        {
            MinSimilarity = newValue;
            return this;
        }

        public IqdbSearchConfiguration AddAllowedRatings(EroRating[] eroRatings)
        {
            if (eroRatings is null)
                throw new ArgumentNullException("Список разрешённых рейтингов не должен быть null", nameof(eroRatings));

            Array.ForEach(eroRatings, x => _eroRatings.Add(x));
            return this;
        }

        public IqdbSearchConfiguration RemoveAllowedRatings(EroRating[] eroRatings)
        {
            if (eroRatings is null)
                throw new ArgumentNullException("Список разрешённых рейтингов не должен быть null", nameof(eroRatings));

            Array.ForEach(eroRatings, x => _eroRatings.Remove(x));
            return this;
        }

        public IqdbSearchConfiguration SetMinimumResolution(Resolution minResolution)
        {
            MinResolution = minResolution;
            return this;
        }

        public IqdbSearchConfiguration AddSitesToSearch(params ImageSite[] imageSites)
        {
            if (imageSites is null)
                throw new ArgumentNullException("Список разрешённых сайтов не должен быть null", nameof(imageSites));

            Array.ForEach(imageSites, x => _sitesToSearch.Add(x));
            return this;
        }

        public IqdbSearchConfiguration RemoveSitesToSearch(params ImageSite[] imageSites)
        {
            if (imageSites is null)
                throw new ArgumentNullException("Список разрешённых сайтов не должен быть null", nameof(imageSites));

            Array.ForEach(imageSites, x => _sitesToSearch.Remove(x));
            return this;
        }

        #endregion

        public static class Factory 
        {
            public static IqdbSearchConfiguration CreateEmpty() 
                => new IqdbSearchConfiguration();
        }
    }
}
