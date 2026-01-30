using ScriptedReviews.Ratings;
using ScriptedReviews.Series;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;

namespace ScriptedReviews.Application.Tests.Ratings
{
    public abstract class RatingAppService_Tests<TStartupModule> : ScriptedReviewsApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IRatingAppService _ratingAppService;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

        private readonly IRepository<Serie, int> _serieRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;

        protected RatingAppService_Tests()
        {
            _ratingAppService = GetRequiredService<IRatingAppService>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
            _serieRepository = GetRequiredService<IRepository<Serie, int>>();
            _userRepository = GetRequiredService<IRepository<IdentityUser, Guid>>();
        }

        [Fact]
        public async Task Should_Rate_Series()
        {

            var fakeUserId = Guid.NewGuid();

            var user = new IdentityUser(fakeUserId, "user_test", "test@email.com");
            await _userRepository.InsertAsync(user, true);

            var claims = new List<Claim>
            {
                new Claim(AbpClaimTypes.UserId, fakeUserId.ToString()),
                new Claim(AbpClaimTypes.UserName, "user_test")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            using (_currentPrincipalAccessor.Change(principal))
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    // Arrange
                    var serie = new Serie
                    {
                        Title = "Test Series",
                        Description = "Descripción de prueba",
                        Image = "test.jpg",
                        Genre = "Acción",
                        Language = "English",
                        ReleaseDate = "2024-01-01",
                        Duration = "45 min",
                        Rating = "8.5", // Rating de IMDB
                        Country = "USA",
                        Director = "Director Test",
                        Cast = "Actor 1, Actor 2",
                        Writer = "Escritor Test",
                        UserId = fakeUserId
                    };

                    await _serieRepository.InsertAsync(serie, true);

                    var input = new CreateRatingDto
                    {
                        SeriesId = serie.Id,
                        RatingNumber = 4,
                        Comment = "Muy buena serie"
                    };

                    // Act
                    var result = await _ratingAppService.RateAsync(input);

                    // Assert
                    result.ShouldNotBeNull();
                    result.RatingNumber.ShouldBe(4);
                    result.Comment.ShouldBe("Muy buena serie");
                    result.SeriesId.ShouldBe(serie.Id);
                    result.UserId.ShouldBe(fakeUserId);
                });
            }
        }
    }
}