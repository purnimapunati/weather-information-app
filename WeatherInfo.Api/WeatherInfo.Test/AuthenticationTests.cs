using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherInfo.Api.Controllers;
using WeatherInfo.Service.Interfaces;
using WeatherInfo.Service.Models;
using Xunit;

namespace WeatherInfo.Test
{
    public class AuthenticationTests
    {
        private readonly Mock<IWeatherService> _mockWeatherService;
        private readonly WeatherController _controller;

        public AuthenticationTests()
        {
            _mockWeatherService = new Mock<IWeatherService>();
            _controller = new WeatherController(_mockWeatherService.Object);
        }

        [Fact]
        public async Task GetWeatherDetails_ValidApiKey_ReturnsSuccess()
        {
            var city = "Melbourne";
            var country = "Australia";
            var expectedWeather = new WeatherResponse { Description = "Clear Sky", StatusCode = 200 };

            _mockWeatherService.Setup(service => service.GetWeatherDetails(city, country))
                .ReturnsAsync(expectedWeather);

            var result = await _controller.GetWeatherDetails(city, country) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task GetWeatherDetails_InvalidApiKey_ReturnsUnauthorized()
        {
            var city = "Melbourne";
            var country = "Australia";
            var expectedWeather = new WeatherResponse { StatusCode = 401 };

            _mockWeatherService.Setup(service => service.GetWeatherDetails(city, country))
                .ReturnsAsync(expectedWeather);

            var result = await _controller.GetWeatherDetails(city, country) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
        }
    }
}
