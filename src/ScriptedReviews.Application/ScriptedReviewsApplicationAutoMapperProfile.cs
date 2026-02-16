using AutoMapper;
using ScriptedReviews.MonitoringLogs;
using ScriptedReviews.Notifications;
using ScriptedReviews.Notifications.Dtos;
using ScriptedReviews.Ratings;
using ScriptedReviews.Series;
using ScriptedReviews.Seasons;
using ScriptedReviews.Watchlists;
using ScriptedReviews.Watchlists.Dtos;

namespace ScriptedReviews;

public class ScriptedReviewsApplicationAutoMapperProfile : Profile
{
    public ScriptedReviewsApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        
        
        CreateMap<Serie, SerieDto>();
        CreateMap<CreateUpdateSerieDto, Serie>();
        CreateMap<Season, SeasonDto>();
        
        //Mapa para Dto de Watchlist
        CreateMap<Watchlist, WatchlistDto>();

        //Mapa para Dto de Notifications
        CreateMap<Notification, NotificationDto>();

        //Mapa para Dto de Rating
        CreateMap<Rating, RatingDto>();
        CreateMap<CreateRatingDto, Rating>();

        //Mapa para Dto de ApiMonitoringLog
        CreateMap<ApiMonitoringLog, ApiMonitoringLogDto>();

        //Mapa para Dto de ErrorLog
        CreateMap<ErrorLogs.ErrorLog, ErrorLogs.ErrorLogDto>();
    }
}
