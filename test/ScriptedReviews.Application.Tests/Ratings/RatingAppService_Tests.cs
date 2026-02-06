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

namespace ScriptedReviews.Ratings
{
    public abstract class RatingAppService_Tests<TStartupModule> : ScriptedReviewsApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IRatingAppService _ratingAppService;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

        private readonly IRepository<Serie, int> _serieRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IRepository<Rating, Guid> _ratingRepository;

        protected RatingAppService_Tests()
        {
            _ratingAppService = GetRequiredService<IRatingAppService>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
            _serieRepository = GetRequiredService<IRepository<Serie, int>>();
            _userRepository = GetRequiredService<IRepository<IdentityUser, Guid>>();
            _ratingRepository = GetRequiredService<IRepository<Rating, Guid>>();
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
                        UserId = fakeUserId,
                        ImdbId = "tt100"
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
            
        [Fact]
        public async Task Should_Update_Rating()
        {
            // Arrange
            var fakeUserId = Guid.NewGuid();

            // Insertamos usuario para evitar error de foreign key
            var user = new IdentityUser(fakeUserId, "updater_user", "update@test.com");
            await _userRepository.InsertAsync(user, true);

            var claims = new List<Claim>
            {
                new Claim(AbpClaimTypes.UserId, fakeUserId.ToString()),
                new Claim(AbpClaimTypes.UserName, "updater_user")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            // Act
            using (_currentPrincipalAccessor.Change(principal))
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    // Creamos la serie con todos sus campos obligatorios
                    var serie = new Serie
                    {
                        Title = "Serie to Update",
                        Description = "Descripción para probar update",
                        UserId = fakeUserId,
                        Image = "test-img.jpg",
                        Genre = "Drama",
                        Language = "Spanish",
                        ReleaseDate = "2024",
                        Duration = "50 min",
                        Rating = "N/A",
                        Country = "Test",
                        Director = "Test",
                        Cast = "Test",
                        Writer = "Test",
                        ImdbId = "Test",
                    };
                    await _serieRepository.InsertAsync(serie, true);

                    // Creamos calificación para luego editarla
                    var initialRating = new Rating
                    {
                        SeriesId = serie.Id,
                        UserId = fakeUserId,
                        RatingNumber = 5,
                        Comment = "Me encantó"
                    };
                    await _ratingRepository.InsertAsync(initialRating, true);

                    // Los datos que vamos a usar para modificar la calificación
                    var updateInput = new UpdateRatingDto
                    {
                        // No pasamos SeriesId aquí porque suele ir en la URL o parámetro aparte
                        RatingNumber = 1,
                        Comment = "Al final no me gustó tanto"
                    };

                    // Llamamos al servicio (Update)
                    var result = await _ratingAppService.UpdateRatingAsync(serie.Id, updateInput);

                    // Assert
                    result.ShouldNotBeNull();
                    result.RatingNumber.ShouldBe(1);
                    result.Comment.ShouldBe("Al final no me gustó tanto");

                    // Opcional: Verificar directamente en base de datos para estar 100% seguros
                    var dbRating = await _ratingRepository.GetAsync(initialRating.Id);
                    dbRating.RatingNumber.ShouldBe(1);
                    dbRating.Comment.ShouldBe("Al final no me gustó tanto");
                });
            }
        }
    }
}