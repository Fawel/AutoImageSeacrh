using System;
using System.Collections.Generic;
using System.Text;

namespace AIS.Application.PictureSearchers.Models
{
    public enum EroRating
    {
        Unknown,
        Safe,
        Questionable,
        Ero,
        Explicit
    }

    public static class EroRatingMethods
    {
        public static EroRating GetRatingFromString(string eroRatingString) =>
            eroRatingString switch
            {
                "Safe" => EroRating.Safe,
                "Questionable" => EroRating.Questionable,
                "Ero" => EroRating.Ero,
                "Explicit" => EroRating.Explicit,
                _ => throw new ArgumentException($"Failed to convert rating {eroRatingString}")
            };
    }
}
