using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Xunit;
using ScriptedReviews.Ratings;
using ScriptedReviews.Series;

namespace ScriptedReviews.Tests
{
    public class RatingAppServiceTests : ScriptedReviewsTestBase<ScriptedReviewsApplicationTestModule>
    {
        private readonly IRatingAppService _ratingAppService;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly ICurrentUser _currentUser;

        public RatingAppServiceTests()
        {
            _ratingAppService = GetRequiredService<IRatingAppService>();
            _serieRepository = GetRequiredService<IRepository<Serie, int>>();
            _currentUser = GetRequiredService<ICurrentUser>();
        }

        [Fact]
        public async Task Should_Rate_Series()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                var serie = new Serie
                {
                    Title = "Test Series",
                    Description = "Test Description",
                    UserId = _currentUser.Id.Value
                };
                await _serieRepository.InsertAsync(serie);
                await _serieRepository.InsertAsync(serie, true); // Guardar cambios en la base de datos

                var input = new CreateRatingDto
                {
                    SeriesId = serie.Id,
                    Rating = 5,
                    Comment = "Great series!"
                };

                // Act
                var result = await _ratingAppService.RateSeriesAsync(input);

                // Assert
                result.ShouldNotBeNull();
                result.Rating.ShouldBe(5);
                result.Comment.ShouldBe("Great series!");
                result.SeriesId.ShouldBe(serie.Id);
                result.UserId.ShouldBe(_currentUser.Id.Value);
            });
        }
    }
}
