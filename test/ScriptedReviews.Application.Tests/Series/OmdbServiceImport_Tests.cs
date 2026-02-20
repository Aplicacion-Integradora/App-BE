using NSubstitute;
using Microsoft.Extensions.Configuration;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;
using Volo.Abp.Modularity;
using Xunit.Abstractions;
using Volo.Abp.Uow;
using System.Net.Http;
using System.Net;
using System.Text.Json;

namespace ScriptedReviews.Series
{
    public abstract class OmdbServiceImport_Tests<TStartupModule> : ScriptedReviewsDomainTestBase<TStartupModule> 
        where TStartupModule : IAbpModule
    {
        private readonly OmdbService _omdbService;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly IConfiguration _configuration;
        private readonly FakeHttpMessageHandler _fakeHttpMessageHandler;

        protected OmdbServiceImport_Tests()
        {
            _omdbService = GetRequiredService<OmdbService>();
            _serieRepository = GetRequiredService<IRepository<Serie, int>>();
            _configuration = GetRequiredService<IConfiguration>();
            _fakeHttpMessageHandler = GetRequiredService<FakeHttpMessageHandler>();
        }

        private void SetupOmdbResponse(string imdbId, object responseObj)
        {
            var json = JsonSerializer.Serialize(responseObj);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
            _fakeHttpMessageHandler.AddResponse($"i={imdbId}", response);
        }

        [Fact]
        public async Task Should_Import_Serie_From_Omdb_And_Save_To_Database()
        {
            // Arrange
            var imdbId = "tt0903747"; // Breaking Bad
            
            // Mock Response
            var mockResponse = new 
            {
                Title = "Breaking Bad",
                imdbID = imdbId,
                Poster = "http://example.com/bb.jpg",
                Genre = "Drama",
                Year = "2008-2013",
                Runtime = "49 min",
                Country = "USA",
                Director = "Vince Gilligan",
                Actors = "Bryan Cranston, Aaron Paul",
                Writer = "Vince Gilligan",
                Plot = "A high school chemistry teacher diagnosed with inoperable lung cancer turns to manufacturing and selling methamphetamine in order to secure his family's future.",
                imdbRating = "9.5",
                totalSeasons = "5",
                Response = "True"
            };
            SetupOmdbResponse(imdbId, mockResponse);
            // Mock Season 1
             _fakeHttpMessageHandler.AddResponse($"i={imdbId}&Season=1", new HttpResponseMessage(HttpStatusCode.OK) { 
                Content = new StringContent(JsonSerializer.Serialize(new { 
                    Title = "Breaking Bad", 
                    Season = "1", 
                    totalSeasons = "5", 
                    Episodes = new[] { new { Title = "Pilot", Released = "2008-01-20", Episode = "1", imdbRating = "9.0", imdbID = "tt1" } },
                    Response = "True"
                })) 
            });


            SerieDto? resultado = null;
            
            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                resultado = await _omdbService.ImportarSerieAsync(imdbId);
            });

            // Assert
            resultado.ShouldNotBeNull("ImportarSerieAsync returned null");
            resultado.Title.ShouldBe("Breaking Bad");
            resultado.ImdbId.ShouldNotBeNullOrEmpty();

            // Verifica que se guardó en la base de datos  
            await WithUnitOfWorkAsync(async () =>
            {
                var serieEnDb = await _serieRepository.FirstOrDefaultAsync(x => x.ImdbId == resultado.ImdbId);
                serieEnDb.ShouldNotBeNull("Serie should be saved to database");
                serieEnDb.Title.ShouldBe("Breaking Bad");
            });
        }

        [Fact]
        public async Task Should_Not_Duplicate_If_Already_Exists()
        {
            // Arrange
            var imdbId = "tt0944947"; // Game of Thrones
            var mockResponse = new 
            {
                Title = "Game of Thrones",
                imdbID = imdbId,
                Poster = "http://example.com/got.jpg",
                Genre = "Fantasy",
                Year = "2011",
                Runtime = "57 min",
                Country = "USA",
                Director = "N/A",
                Actors = "Emilia Clarke",
                Writer = "George R.R. Martin",
                Plot = "Nine noble families fight for control over the lands of Westeros.",
                imdbRating = "9.3",
                totalSeasons = "8",
                Response = "True"
            };
            SetupOmdbResponse(imdbId, mockResponse);
            // Mock Season 1 just in case
             _fakeHttpMessageHandler.AddResponse($"i={imdbId}&Season=1", new HttpResponseMessage(HttpStatusCode.OK) { 
                Content = new StringContent(JsonSerializer.Serialize(new { 
                    Title = "GOT", Season = "1", totalSeasons = "8", 
                    Episodes = new[] { new { Title = "Winter is Coming", Released = "2011-04-17", Episode = "1", imdbRating = "9.0", imdbID = "tt2" } },
                    Response = "True"
                })) 
            });


            SerieDto? primeraImportacion = null;
            SerieDto? segundaImportacion = null;
            long cantidadInicial = 0;
            long cantidadFinal = 0;

            // Act
            // Primer import
            await WithUnitOfWorkAsync(async () =>
            {
                primeraImportacion = await _omdbService.ImportarSerieAsync(imdbId);
            });
            
            primeraImportacion.ShouldNotBeNull("First import returned null");
            
            await WithUnitOfWorkAsync(async () =>
            {
                cantidadInicial = await _serieRepository.GetCountAsync();
            });

            // Segundo import (deberia devolver la serie existente)
            await WithUnitOfWorkAsync(async () =>
            {
                segundaImportacion = await _omdbService.ImportarSerieAsync(imdbId);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                cantidadFinal = await _serieRepository.GetCountAsync();
            });

            // Assert
            segundaImportacion.ShouldNotBeNull("Segundo import debería devolver serie existente");
            cantidadFinal.ShouldBe(cantidadInicial);
            segundaImportacion.ImdbId.ShouldBe(primeraImportacion.ImdbId);
        }

        [Fact]
        public async Task Should_Throw_When_Serie_Not_Found()
        {
            // Arrange
            var idInexistente = "tt000000000";
             _fakeHttpMessageHandler.AddResponse($"i={idInexistente}", new HttpResponseMessage(HttpStatusCode.OK) { 
                Content = new StringContent(JsonSerializer.Serialize(new { Response = "False", Error = "Incorrect IMDb ID." })) 
            });


            // Act & Assert - debería lanzar UserFriendlyException
            await Assert.ThrowsAsync<Volo.Abp.UserFriendlyException>(async () =>
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    await _omdbService.ImportarSerieAsync(idInexistente);
                });
            });
        }

        [Fact]
        public async Task Should_Store_All_Required_Fields()
        {
            // Arrange
            var imdbId = "tt0386676"; // The Office
             var mockResponse = new 
            {
                Title = "The Office",
                imdbID = imdbId,
                Poster = "http://example.com/office.jpg",
                Genre = "Comedy",
                Year = "2005",
                Runtime = "22 min",
                Country = "USA",
                Director = "Greg Daniels",
                Actors = "Steve Carell",
                Writer = "Greg Daniels",
                Plot = "A mockumentary on a group of typical office workers.",
                imdbRating = "8.9",
                totalSeasons = "9",
                Response = "True"
            };
            SetupOmdbResponse(imdbId, mockResponse);
             // Mock Season 1
             _fakeHttpMessageHandler.AddResponse($"i={imdbId}&Season=1", new HttpResponseMessage(HttpStatusCode.OK) { 
                Content = new StringContent(JsonSerializer.Serialize(new { 
                    Title = "The Office", Season = "1", totalSeasons = "9", 
                    Episodes = new[] { new { Title = "Pilot", Released = "2005-03-24", Episode = "1", imdbRating = "7.5", imdbID = "tt3" } },
                    Response = "True"
                })) 
            });

            SerieDto? resultado = null;

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                resultado = await _omdbService.ImportarSerieAsync(imdbId);
            });

            // Assert
            resultado.ShouldNotBeNull("El resultado no debería ser nulo");
            resultado.Title.ShouldNotBeNullOrEmpty();
            resultado.ImdbId.ShouldNotBeNullOrEmpty();

            // Verifica en la base de datos
            await WithUnitOfWorkAsync(async () =>
            {
                var serieEnDb = await _serieRepository.FirstOrDefaultAsync(x => x.ImdbId == resultado.ImdbId);
                serieEnDb.ShouldNotBeNull();
                serieEnDb.Genre.ShouldNotBeNullOrEmpty();
                serieEnDb.Language.ShouldNotBeNullOrEmpty();
            });
        }
    }
}
