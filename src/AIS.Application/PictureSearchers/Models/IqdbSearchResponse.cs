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
            public static IqdbSearchResponse Create(IIqdbImageSearchResult bestMatch,
                                                    IIqdbImageSearchResult[] otherMatches)
            {
                return new IqdbSearchResponse(bestMatch, otherMatches);
            }
        }
    }
}
