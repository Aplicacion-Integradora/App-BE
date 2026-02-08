using ScriptedReviews.Seasons;
using ScriptedReviews.Series;
using ScriptedReviews.Watchlists;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using Xunit;

namespace ScriptedReviews.Watchlists;

public abstract class WatchlistAppService_Tests<TStartupModule> : ScriptedReviewsApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IWatchlistAppService _watchlistAppService;
    private readonly IRepository<Watchlist, int> _watchlistRepository;
    private readonly IRepository<Serie, int> _serieRepository;
    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

    protected WatchlistAppService_Tests()
    {
        _watchlistAppService = GetRequiredService<IWatchlistAppService>();
        _watchlistRepository = GetRequiredService<IRepository<Watchlist, int>>();
        _serieRepository = GetRequiredService<IRepository<Serie, int>>();
        _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
    }

    [Fact]
    public async Task Should_Get_Empty_Watchlist_For_New_User()
    {
        // Arrange
        var myUserId = Guid.NewGuid();

        // Act
        List<SerieDto> result;
        using (_currentPrincipalAccessor.Change(GetClaims(myUserId)))
        {
            result = await _watchlistAppService.GetMyWatchlistAsync();
        }

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_Get_Series_In_Watchlist()
    {
        // Arrange
        var myUserId = Guid.NewGuid();
        var serieId = 0;

        // Create a serie and a watchlist with that serie
        await WithUnitOfWorkAsync(async () =>
        {
            var serie = await _serieRepository.InsertAsync(new Serie
            {
                Title = "Breaking Bad",
                ImdbId = "tt0903747",
                Description = "Un profesor de química con cáncer...",
                Image = "bb.jpg",
                Genre = "Drama",
                Language = "English",
                ReleaseDate = "2008-01-20",
                Duration = "45 min",
                Rating = "9.5",
                Country = "USA",
                Director = "Vince Gilligan",
                Cast = "Bryan Cranston, Aaron Paul",
                Writer = "Vince Gilligan",
                UserId = myUserId,
                Seasons = new List<Season>()
            }, autoSave: true);

            serieId = serie.Id;

            var watchlist = new Watchlist(myUserId)
            {
                Name = "Mi Lista"
            };
            watchlist.Series = new List<Serie> { serie };
            await _watchlistRepository.InsertAsync(watchlist, autoSave: true);
        });

        // Act
        List<SerieDto> result;
        using (_currentPrincipalAccessor.Change(GetClaims(myUserId)))
        {
            result = await _watchlistAppService.GetMyWatchlistAsync();
        }

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result[0].Title.ShouldBe("Breaking Bad");
        result[0].Id.ShouldBe(serieId);
    }

    [Fact]
    public async Task Should_Add_Serie_To_Watchlist()
    {
        // Arrange
        var myUserId = Guid.NewGuid();
        var serieId = 0;

        // Create a serie without adding to watchlist
        await WithUnitOfWorkAsync(async () =>
        {
            var serie = await _serieRepository.InsertAsync(new Serie
            {
                Title = "Game of Thrones",
                ImdbId = "tt0944947",
                Description = "Familias nobles luchan por el trono...",
                Image = "got.jpg",
                Genre = "Fantasy",
                Language = "English",
                ReleaseDate = "2011-04-17",
                Duration = "60 min",
                Rating = "9.3",
                Country = "USA",
                Director = "Alan Taylor",
                Cast = "Emilia Clarke, Kit Harington",
                Writer = "George R.R. Martin",
                UserId = myUserId,
                Seasons = new List<Season>()
            }, autoSave: true);

            serieId = serie.Id;
        });

        // Act
        using (_currentPrincipalAccessor.Change(GetClaims(myUserId)))
        {
            await _watchlistAppService.AddSerieAsync(serieId);
        }

        // Assert - verify the serie is now in the watchlist
        List<SerieDto> result;
        using (_currentPrincipalAccessor.Change(GetClaims(myUserId)))
        {
            result = await _watchlistAppService.GetMyWatchlistAsync();
        }

        result.ShouldNotBeNull();
        result.Count.ShouldBe(1);
        result.ShouldContain(s => s.Title == "Game of Thrones");
    }

    [Fact]
    public async Task Should_Not_Add_Duplicate_Serie_To_Watchlist()
    {
        // Arrange
        var myUserId = Guid.NewGuid();
        var serieId = 0;

        await WithUnitOfWorkAsync(async () =>
        {
            var serie = await _serieRepository.InsertAsync(new Serie
            {
                Title = "The Mandalorian",
                ImdbId = "tt8111088",
                Description = "Un cazarrecompensas solitario...",
                Image = "mando.jpg",
                Genre = "Sci-Fi",
                Language = "English",
                ReleaseDate = "2019-11-12",
                Duration = "40 min",
                Rating = "8.8",
                Country = "USA",
                Director = "Jon Favreau",
                Cast = "Pedro Pascal",
                Writer = "Jon Favreau",
                UserId = myUserId,
                Seasons = new List<Season>()
            }, autoSave: true);

            serieId = serie.Id;

            var watchlist = new Watchlist(myUserId)
            {
                Name = "Mi Lista"
            };
            watchlist.Series = new List<Serie> { serie };
            await _watchlistRepository.InsertAsync(watchlist, autoSave: true);
        });

        // Act & Assert - should throw when trying to add duplicate
        using (_currentPrincipalAccessor.Change(GetClaims(myUserId)))
        {
            var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                await _watchlistAppService.AddSerieAsync(serieId);
            });

            exception.Message.ShouldContain("ya está en tu lista");
        }
    }

    [Fact]
    public async Task Should_Remove_Serie_From_Watchlist()
    {
        // Arrange
        var myUserId = Guid.NewGuid();
        var serieId = 0;

        await WithUnitOfWorkAsync(async () =>
        {
            var serie = await _serieRepository.InsertAsync(new Serie
            {
                Title = "Stranger Things",
                ImdbId = "tt4574334",
                Description = "Un grupo de niños enfrenta fuerzas sobrenaturales...",
                Image = "st.jpg",
                Genre = "Sci-Fi",
                Language = "English",
                ReleaseDate = "2016-07-15",
                Duration = "50 min",
                Rating = "8.7",
                Country = "USA",
                Director = "Duffer Brothers",
                Cast = "Millie Bobby Brown",
                Writer = "Duffer Brothers",
                UserId = myUserId,
                Seasons = new List<Season>()
            }, autoSave: true);

            serieId = serie.Id;

            var watchlist = new Watchlist(myUserId)
            {
                Name = "Mi Lista"
            };
            watchlist.Series = new List<Serie> { serie };
            await _watchlistRepository.InsertAsync(watchlist, autoSave: true);
        });

        // Verify setup - serie should be in watchlist initially
        List<SerieDto> beforeRemove;
        using (_currentPrincipalAccessor.Change(GetClaims(myUserId)))
        {
            beforeRemove = await _watchlistAppService.GetMyWatchlistAsync();
        }
        beforeRemove.Count.ShouldBe(1);

        // Act - remove the serie
        using (_currentPrincipalAccessor.Change(GetClaims(myUserId)))
        {
            await _watchlistAppService.RemoveSerieAsync(serieId);
        }

        // Assert - watchlist should be empty now
        List<SerieDto> afterRemove;
        using (_currentPrincipalAccessor.Change(GetClaims(myUserId)))
        {
            afterRemove = await _watchlistAppService.GetMyWatchlistAsync();
        }

        afterRemove.ShouldNotBeNull();
        afterRemove.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_Only_See_Own_Watchlist()
    {
        // Arrange
        var user1Id = Guid.NewGuid();
        var user2Id = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            // Create series for user 1
            var serie1 = await _serieRepository.InsertAsync(new Serie
            {
                Title = "User1 Serie",
                ImdbId = "tt1111111",
                Description = "Serie del usuario 1",
                Image = "u1.jpg",
                Genre = "Drama",
                Language = "English",
                ReleaseDate = "2020-01-01",
                Duration = "45 min",
                Rating = "9.0",
                Country = "USA",
                Director = "Director 1",
                Cast = "Cast 1",
                Writer = "Writer 1",
                UserId = user1Id,
                Seasons = new List<Season>()
            }, autoSave: true);

            var watchlist1 = new Watchlist(user1Id) { Name = "Lista User1" };
            watchlist1.Series = new List<Serie> { serie1 };
            await _watchlistRepository.InsertAsync(watchlist1, autoSave: true);

            // Create series for user 2
            var serie2 = await _serieRepository.InsertAsync(new Serie
            {
                Title = "User2 Serie",
                ImdbId = "tt2222222",
                Description = "Serie del usuario 2",
                Image = "u2.jpg",
                Genre = "Comedy",
                Language = "English",
                ReleaseDate = "2021-01-01",
                Duration = "30 min",
                Rating = "8.0",
                Country = "USA",
                Director = "Director 2",
                Cast = "Cast 2",
                Writer = "Writer 2",
                UserId = user2Id,
                Seasons = new List<Season>()
            }, autoSave: true);

            var watchlist2 = new Watchlist(user2Id) { Name = "Lista User2" };
            watchlist2.Series = new List<Serie> { serie2 };
            await _watchlistRepository.InsertAsync(watchlist2, autoSave: true);
        });

        // Act - user 1 should only see their own series
        List<SerieDto> user1Result;
        using (_currentPrincipalAccessor.Change(GetClaims(user1Id)))
        {
            user1Result = await _watchlistAppService.GetMyWatchlistAsync();
        }

        // Assert
        user1Result.ShouldNotBeNull();
        user1Result.Count.ShouldBe(1);
        user1Result[0].Title.ShouldBe("User1 Serie");
        user1Result.ShouldNotContain(s => s.Title == "User2 Serie");
    }

    private ClaimsPrincipal GetClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new Claim(AbpClaimTypes.UserId, userId.ToString()),
            new Claim(AbpClaimTypes.UserName, "user_test")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        return new ClaimsPrincipal(identity);
    }
}
