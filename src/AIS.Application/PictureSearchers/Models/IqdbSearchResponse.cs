using System;
using System.Collections.Generic;
using System.Text;

namespace AIS.Application.PictureSearchers.Models
{
    public class IqdbSearchResponse
    {
        public IIqdbImageSearchResult BestMatch { get; private set; }
        public IIqdbImageSearchResult[] OtherMatches { get; private set; } = new IIqdbImageSearchResult[0];

        private IqdbSearchResponse(IIqdbImageSearchResult bestMatch,
                                   IIqdbImageSearchResult[] otherMatches)
        {
            SetBestMatch(bestMatch)
                .SetOtherMatches(otherMatches);
        }

        private IqdbSearchResponse SetBestMatch(IIqdbImageSearchResult newBestMatch)
        {
            if (newBestMatch is null)
                throw new ArgumentNullException(nameof(newBestMatch));

            BestMatch = newBestMatch;
            return this;
        }

        private IqdbSearchResponse SetOtherMatches(IIqdbImageSearchResult[] newOtherMatches)
        {
            if (newOtherMatches is null)
                throw new ArgumentNullException(nameof(newOtherMatches));

            OtherMatches = newOtherMatches;
            return this;
        }

        public static class Factory
        {
            public static IqdbSearchResponse CreatePictureFoundResponse(IIqdbImageSearchResult bestMatch,
                                                    IIqdbImageSearchResult[] otherMatches)
            {
                return new IqdbSearchResponse(bestMatch, otherMatches);
            }

            /// <summary>
            /// Создаём модель ответа, когда картинка не была найдена
            /// </summary>
            /// <param name="additionalMatches">Список найденных возможно похожих картинок, может быть null</param>
            /// <returns>Объект ответа поиска iqdb, где искомая картинка не была найдена</returns>
            public static IqdbSearchResponse CreateNotFoundResponse(IIqdbImageSearchResult[] additionalMatches = null)
            {
                additionalMatches = additionalMatches ?? new IIqdbImageSearchResult[0];
                var failedSearch = new IqdbFailedSearch();
                return new IqdbSearchResponse(failedSearch, additionalMatches);
            }

            /// <summary>
            /// Создаём модель ответа, когда Iqdb не смог обработать файл
            /// </summary>
            /// <returns>Объект ответа поиска iqdb, где запрос не был обработан</returns>
            public static IqdbSearchResponse CreateBadQueryResponse()
            {
                var badQuerySearch = new IqdbBadQuerySearch();
                return new IqdbSearchResponse(badQuerySearch, new IIqdbImageSearchResult[0]);
            }
        }
    }
}
