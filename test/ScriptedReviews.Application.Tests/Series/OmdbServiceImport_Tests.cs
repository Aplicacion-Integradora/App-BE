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

namespace ScriptedReviews.Series
{
    public abstract class OmdbServiceImport_Tests<TStartupModule> : ScriptedReviewsDomainTestBase<TStartupModule> 
        where TStartupModule : IAbpModule
    {
        private readonly OmdbService _omdbService;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly IConfiguration _configuration;

        protected OmdbServiceImport_Tests()
        {
            _omdbService = GetRequiredService<OmdbService>();
            _serieRepository = GetRequiredService<IRepository<Serie, int>>();
            _configuration = GetRequiredService<IConfiguration>();
        }

        [Fact]
        public async Task Should_Import_Serie_From_Omdb_And_Save_To_Database()
        {
            // Arrange
            var titulo = "Breaking Bad";
            
            var apiKey = _configuration["OmdbApiKey"];
            apiKey.ShouldNotBeNullOrEmpty("OmdbApiKey should be configured in appsettings.json");

            SerieDto? resultado = null;
            
            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                resultado = await _omdbService.ImportarSerieAsync(titulo);
            });

            // Assert
            resultado.ShouldNotBeNull("ImportarSerieAsync returned null");
            resultado.Title.ShouldBe("Breaking Bad");
            resultado.ImdbId.ShouldNotBeNullOrEmpty();

            // Verificar que se guardó en la base de datos  
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
            var titulo = "Game of Thrones";
            SerieDto? primeraImportacion = null;
            SerieDto? segundaImportacion = null;
            long cantidadInicial = 0;
            long cantidadFinal = 0;

            // Act
            // Primer import
            await WithUnitOfWorkAsync(async () =>
            {
                primeraImportacion = await _omdbService.ImportarSerieAsync(titulo);
            });
            
            primeraImportacion.ShouldNotBeNull("First import returned null");
            
            await WithUnitOfWorkAsync(async () =>
            {
                cantidadInicial = await _serieRepository.GetCountAsync();
            });

            // Segundo import (deberia devolver la serie existente)
            await WithUnitOfWorkAsync(async () =>
            {
                segundaImportacion = await _omdbService.ImportarSerieAsync(titulo);
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
            var tituloInexistente = "xyzabc123nonexistent_series_title_9999";

            // Act & Assert - debería lanzar UserFriendlyException
            await Assert.ThrowsAsync<Volo.Abp.UserFriendlyException>(async () =>
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    await _omdbService.ImportarSerieAsync(tituloInexistente);
                });
            });
        }

        [Fact]
        public async Task Should_Store_All_Required_Fields()
        {
            // Arrange
            var titulo = "The Office";
            SerieDto? resultado = null;

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                resultado = await _omdbService.ImportarSerieAsync(titulo);
            });

            // Assert
            resultado.ShouldNotBeNull("El resultado no debería ser nulo");
            resultado.Title.ShouldNotBeNullOrEmpty();
            resultado.ImdbId.ShouldNotBeNullOrEmpty();

            // Verificar en la base de datos
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
