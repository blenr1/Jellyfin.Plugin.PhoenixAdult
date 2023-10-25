using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;

namespace PhoenixAdult.Models
{
public static class MasterVideo
    {
        public static Episode VideoToEpisode(Video video)
        {
            var episode = new Episode();
            episode.Tags = video.Tags;
            episode.Tagline = video.Tagline;
            episode.Genres = video.Genres;
            episode.ProviderIds = video.ProviderIds;
            episode.ProductionYear = video.ProductionYear;
            episode.ImageInfos = video.ImageInfos;
            episode.OriginalTitle = video.OriginalTitle;
            episode.PremiereDate = video.PremiereDate;
            episode.Studios = video.Studios;
            episode.Overview = video.Overview;
            episode.HomePageUrl = video.HomePageUrl;
            episode.Name = video.Name;
            return episode;
        }

        public static Movie VideoToMovie(Video video)
        {
            var movie = new Movie();
            movie.Tags = video.Tags;
            movie.Tagline = video.Tagline;
            movie.Genres = video.Genres;
            movie.ProviderIds = video.ProviderIds;
            movie.ProductionYear = video.ProductionYear;
            movie.ImageInfos = video.ImageInfos;
            movie.OriginalTitle = video.OriginalTitle;
            movie.PremiereDate = video.PremiereDate;
            movie.Studios = video.Studios;
            movie.Overview = video.Overview;
            movie.HomePageUrl = video.HomePageUrl;
            movie.Name = video.Name;

            return movie;
        }
    }
}
