using AutoMapper;
using ScriptedReviews.MonitoringLogs;
using ScriptedReviews.Series;
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

        CreateMap<Watchlist, WatchlistDto>();

        CreateMap<ApiMonitoringLog, ApiMonitoringLogDto>();
    }
}
