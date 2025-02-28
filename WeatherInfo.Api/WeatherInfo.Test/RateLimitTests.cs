using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherInfo.Api.Controllers;
using WeatherInfo.Service.Interfaces;
using WeatherInfo.Service.Models;
using Xunit;

namespace WeatherInfo.Test
{
    public class RateLimitTests
    {
        private readonly Mock<IWeatherService> _mockWeatherService;
        private readonly WeatherController _controller;
        private int _requestCount = 0;

        public RateLimitTests()
        {
            _mockWeatherService = new Mock<IWeatherService>();
            _controller = new WeatherController(_mockWeatherService.Object);
        }

        [Fact]
        public async Task RateLimit_WithinLimit_ReturnsOk()
        {
            var city = "Melbourne";
            var country = "Australia";
            var expectedWeather = new WeatherResponse { Description = "Cloudy", StatusCode = 200 };

            _mockWeatherService.Setup(service => service.GetWeatherDetails(city, country))
                .ReturnsAsync(expectedWeather);

            for (int i = 0; i < 5; i++)
            {
                var result = await _controller.GetWeatherDetails(city, country) as ObjectResult;
                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
            }
        }

        [Fact]
        public async Task RateLimit_Exceeded_ReturnsTooManyRequests()
        {
            var city = "Melbourne";
            var country = "Australia";

            _mockWeatherService.Setup(service => service.GetWeatherDetails(city, country))
                .ReturnsAsync(() =>
                {
                    _requestCount++;
                    if (_requestCount > 5)
                    {
                        return new WeatherResponse { StatusCode = 429, ErrorMessage = "Rate limit exceeded" };
                    }
                    return new WeatherResponse { Description = "Cloudy", StatusCode = 200 };
                });

            for (int i = 0; i < 5; i++)
            {
                var result = await _controller.GetWeatherDetails(city, country) as ObjectResult;
                Assert.NotNull(result);
                Assert.Equal(200, result.StatusCode);
            }

            var rateLimitedResult = await _controller.GetWeatherDetails(city, country) as ObjectResult;
            Assert.NotNull(rateLimitedResult);
            Assert.Equal(429, ((WeatherResponse)rateLimitedResult.Value).StatusCode);
            Assert.Equal("Rate limit exceeded", ((WeatherResponse)rateLimitedResult.Value).ErrorMessage);
        }
    }
}
