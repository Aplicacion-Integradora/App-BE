using NSubstitute;
using Microsoft.Extensions.Configuration;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;
using Volo.Abp.Modularity;
using Volo.Abp.Application.Dtos;

namespace ScriptedReviews.Series
{
    public abstract class SerieAppService_Tests<TStartupModule> : ScriptedReviewsDomainTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly ISerieAppService _serieAppService;
        private readonly IRepository<Serie, int> _serieRepository;

        protected SerieAppService_Tests()
        {
            _serieAppService = GetRequiredService<ISerieAppService>();
            _serieRepository = GetRequiredService<IRepository<Serie, int>>();
        }

        [Fact]
        public async Task Should_Get_Serie_By_Id()
        {
            // Arrange - Crear una serie en la base de datos
            Serie serieCreada = null!;
            await WithUnitOfWorkAsync(async () =>
            {
                var serie = new Serie
                {
                    Title = "Test Serie",
                    ImdbId = "tt9999999",
                    Genre = "Drama",
                    ReleaseDate = "2024",
                    Description = "Test description",
                    Language = "English",
                    Country = "USA",
                    Director = "Test Director",
                    Cast = "Test Cast",
                    Writer = "Test Writer",
                    Duration = "45 min",
                    Rating = "8.5",
                    TotalSeasons = 3,
                    Image = "https://example.com/test.jpg"
                };
                serieCreada = await _serieRepository.InsertAsync(serie, autoSave: true);
            });

            // Act - Obtener la serie por ID
            var resultado = await _serieAppService.GetAsync(serieCreada.Id);

            // Assert
            resultado.ShouldNotBeNull();
            resultado.Id.ShouldBe(serieCreada.Id);
            resultado.Title.ShouldBe("Test Serie");
            resultado.ImdbId.ShouldBe("tt9999999");
            resultado.Genre.ShouldBe("Drama");
        }

        [Fact]
        public async Task Should_Get_List_Of_Series()
        {
            // Arrange - Crear varias series en la base de datos
            await WithUnitOfWorkAsync(async () =>
            {
                await _serieRepository.InsertAsync(new Serie
                {
                    Title = "Serie A",
                    ImdbId = "tt1111111",
                    Genre = "Comedy",
                    ReleaseDate = "2020",
                    Description = "Comedy series",
                    Language = "English",
                    Country = "USA",
                    Director = "Director A",
                    Cast = "Cast A",
                    Writer = "Writer A",
                    Duration = "30 min",
                    Rating = "7.5",
                    TotalSeasons = 2,
                    Image = "https://example.com/serieA.jpg"
                });

                await _serieRepository.InsertAsync(new Serie
                {
                    Title = "Serie B",
                    ImdbId = "tt2222222",
                    Genre = "Action",
                    ReleaseDate = "2021",
                    Description = "Action series",
                    Language = "Spanish",
                    Country = "Spain",
                    Director = "Director B",
                    Cast = "Cast B",
                    Writer = "Writer B",
                    Duration = "60 min",
                    Rating = "8.0",
                    TotalSeasons = 1,
                    Image = "https://example.com/serieB.jpg"
                }, autoSave: true);
            });

            // Act - Obtener la lista de series
            var resultado = await _serieAppService.GetListAsync(new PagedAndSortedResultRequestDto());

            // Assert
            resultado.ShouldNotBeNull();
            resultado.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
            resultado.Items.ShouldContain(s => s.Title == "Serie A");
            resultado.Items.ShouldContain(s => s.Title == "Serie B");
        }

        [Fact]
        public async Task Should_Return_Empty_When_No_Series_Exist()
        {
            // Act - Obtener lista cuando no hay series (o solo las creadas por otros tests)
            var resultado = await _serieAppService.GetListAsync(new PagedAndSortedResultRequestDto 
            { 
                MaxResultCount = 1000 
            });

            // Assert
            resultado.ShouldNotBeNull();
            resultado.Items.ShouldNotBeNull();
        }

        [Fact]
        public async Task Should_Return_Correct_Fields_When_Getting_Serie()
        {
            // Arrange - Crear una serie con todos los campos
            Serie serieCreada = null!;
            await WithUnitOfWorkAsync(async () =>
            {
                var serie = new Serie
                {
                    Title = "Complete Serie",
                    ImdbId = "tt8888888",
                    Genre = "Sci-Fi",
                    ReleaseDate = "2023",
                    Description = "A complete test series",
                    Language = "English",
                    Country = "UK",
                    Director = "Famous Director",
                    Cast = "Actor 1, Actor 2",
                    Writer = "Writer 1, Writer 2",
                    Duration = "50 min",
                    Rating = "9.0",
                    TotalSeasons = 5,
                    Image = "https://example.com/poster.jpg"
                };
                serieCreada = await _serieRepository.InsertAsync(serie, autoSave: true);
            });

            // Act
            var resultado = await _serieAppService.GetAsync(serieCreada.Id);

            // Assert - Verificar todos los campos importantes
            resultado.ShouldNotBeNull();
            resultado.Title.ShouldBe("Complete Serie");
            resultado.Genre.ShouldBe("Sci-Fi");
            resultado.ReleaseDate.ShouldBe("2023");
            resultado.Description.ShouldBe("A complete test series");
            resultado.Language.ShouldBe("English");
            resultado.Country.ShouldBe("UK");
            resultado.Director.ShouldBe("Famous Director");
            resultado.Cast.ShouldBe("Actor 1, Actor 2");
            resultado.Writer.ShouldBe("Writer 1, Writer 2");
            resultado.Duration.ShouldBe("50 min");
            resultado.Rating.ShouldBe("9.0");
            resultado.TotalSeasons.ShouldBe(5);
        }
    }
}