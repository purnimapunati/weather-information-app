using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherInfo.Api.Controllers;
using WeatherInfo.Service.Interfaces;
using WeatherInfo.Service.Models;
using Xunit;

namespace WeatherInfo.Test
{
    public class WeatherControllerTest
    {
        private readonly Mock<IWeatherService> _mockWeatherService;
        private readonly WeatherController _controller;

        public WeatherControllerTest()
        {
            _mockWeatherService = new Mock<IWeatherService>();
            _controller = new WeatherController(_mockWeatherService.Object);
        }

        [Fact]
        public async Task GetWeatherDetails_ValidRequest_Returns_Ok()
        {
            var expectedWeather = new WeatherResponse { Description = "Cloudy", StatusCode = 200 };
            _mockWeatherService.Setup(service => service.GetWeatherDetails("Melbourne", "Australia"))
                .ReturnsAsync(expectedWeather);

            var result = await _controller.GetWeatherDetails("Melbourne", "Australia") as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Cloudy", ((WeatherResponse)result.Value).Description);
        }

        [Fact]
        public async Task GetWeatherDetails_InvalidCity_ReturnsNotFound()
        {
            var expectedWeather = new WeatherResponse { ErrorMessage = "city not found", StatusCode = 404 };
            _mockWeatherService.Setup(service => service.GetWeatherDetails("invalidCity", "Australia"))
                .ReturnsAsync(expectedWeather);

            var result = await _controller.GetWeatherDetails("invalidCity", "Australia") as NotFoundObjectResult;

            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task GetWeatherDetails_EmptyCity_ReturnsBadRequest()
        {
            var expectedWeather = new WeatherResponse { ErrorMessage = "City is required", StatusCode = 400 };
            _mockWeatherService.Setup(service => service.GetWeatherDetails("", "Australia"))
                .ReturnsAsync(expectedWeather);

            var result = await _controller.GetWeatherDetails("", "Australia") as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }
    }
}
