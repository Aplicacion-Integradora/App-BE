using NSubstitute;
using Microsoft.Extensions.Configuration;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;
using System.Net.Http;
using System.Net;

namespace ScriptedReviews.Series
{
    public abstract class OmdbService_Tests<TStartupModule> : ScriptedReviewsDomainTestBase<TStartupModule> where TStartupModule : IAbpModule
    {
        private readonly OmdbService _service;
        private readonly FakeHttpMessageHandler _fakeHttpMessageHandler;

        protected OmdbService_Tests()
        {
            var configuration = Substitute.For<IConfiguration>();
            configuration["OMDB:ApiKey"].Returns("12345"); // Use consistent key format
            
            var serieRepository = Substitute.For<IRepository<Serie, int>>();

            _fakeHttpMessageHandler = new FakeHttpMessageHandler();
            var httpClient = new HttpClient(_fakeHttpMessageHandler)
            {
                BaseAddress = new Uri("http://www.omdbapi.com/")
            };

            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

            _service = new OmdbService(configuration, serieRepository, httpClientFactory);
        }

        private void SetupSearchResponse(string title, object responseObj)
        {
            var json = JsonSerializer.Serialize(responseObj);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
            // OmdbService query: ?s={title}&apikey=...
            _fakeHttpMessageHandler.AddResponse($"s={title}", response);
        }

        [Fact]
        public async Task Should_Search_A_Serie()
        {
            //Arrange
            var title = "Game of Thrones";
            var mockResponse = new 
            {
                Search = new[] 
                {
                    new { Title = "Game of Thrones", imdbID = "tt0944947", Poster = "N/A", Year = "2011", Type = "series" }
                },
                totalResults = "1",
                Response = "True"
            };
            SetupSearchResponse(title, mockResponse);

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
            var mockResponse = new 
            {
                Response = "False",
                Error = "Movie not found!"
            };
            SetupSearchResponse(title, mockResponse);

            //Act
            var result = await _service.GetSeriesAsync(title, String.Empty);

            //Assert
            result.Count.ShouldBe(0);
        }
    }
}
