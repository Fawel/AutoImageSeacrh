using System;

namespace AIS.Application.PictureSearchers.Models
{
    public enum ImagePoolSite
    {
        Danbooru,
        Gelbooru,
        Konachan,
        yande_re,
        Sankaku_Channel,
        e_shuushuu,
        Zerochan,
        Anime_Pictures
    }

    public static class ImagePoolSiteMethods
    {
        public static ImagePoolSite GetPoolSiteFromUri(Uri uri) =>
            uri.Host.ToLower() switch
            {
                "danbooru.donmai.us" => ImagePoolSite.Danbooru,
                "gelbooru.com" => ImagePoolSite.Gelbooru,
                "konachan.com" => ImagePoolSite.Konachan,
                "yande.re" => ImagePoolSite.yande_re,
                "chan.sankakucomplex.com" => ImagePoolSite.Sankaku_Channel,
                "e-shuushuu.net" => ImagePoolSite.e_shuushuu,
                "www.zerochan.net" => ImagePoolSite.Zerochan,
                "anime-pictures.net" => ImagePoolSite.Anime_Pictures,
                _ => throw new ArgumentException($"Unknown domain {uri.Host}", nameof(uri))
            };
    }
}
