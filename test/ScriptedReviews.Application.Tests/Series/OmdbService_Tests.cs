using NSubstitute;
using Microsoft.Extensions.Configuration;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace ScriptedReviews.Series
{
        public abstract class OmdbService_Tests<TStartupModule> : ScriptedReviewsDomainTestBase<TStartupModule> where TStartupModule : IAbpModule
        {
            private readonly OmdbService _service;

            protected OmdbService_Tests()
            {
            // 1. Creamos el Mock de la Configuración (simulamos el appsettings)
            var configuration = Substitute.For<IConfiguration>();
            // Le decimos: "Cuando te pidan la API Key, devuelve '12345'"
            configuration["OmdbApiKey"].Returns("12345_clave_falsa");

            // 2. Creamos el Mock del Repositorio (simulamos la base de datos)
            var serieRepository = Substitute.For<IRepository<Serie, int>>();

            // 3. AHORA SÍ: Instanciamos el servicio pasándole los mocks
            _service = new OmdbService(configuration, serieRepository);
        }

            [Fact]
            public async Task Should_Search_A_Serie()
            {
                //Arrange
                var title = "Game of Thrones";

                //Act
                var result = await _service.GetSeriesAsync(title, String.Empty);

                //Assert
                result.Count.ShouldBeGreaterThan(0);
                result.ShouldContain(b => b.Title == title);
            }

            [Fact]
            public async Task Should_Search_None_Serie()
            {
                //Arrange
                var title = "guarioehnuigaoe";

                //Act
                var result = await _service.GetSeriesAsync(title, String.Empty);

                //Assert
                result.Count.ShouldBe(0);
            }
        }
}
