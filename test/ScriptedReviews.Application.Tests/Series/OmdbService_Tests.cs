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
            var configuration = Substitute.For<IConfiguration>();
            configuration["OmdbApiKey"].Returns("12345_clave_falsa");
            var serieRepository = Substitute.For<IRepository<Serie, int>>();

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
