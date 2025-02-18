using AutoMapper;
using ScriptedReviews.Series;

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

    }
}
