using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoggerHealthCheckExampleWithSerilog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("kaboom/1")]
        public IEnumerable<WeatherForecast> Kaboom1()
        {
            throw new ArgumentException("You are wrong");
        }

        [HttpGet("kaboom/2")]
        public ActionResult Kaboom2()
        {
            Test.Explode();
            return Ok();
        }



        [HttpGet("kaboom/3")]
        public ActionResult Kaboom3()
        {
            logger.LogWarning("Warning");
            return Ok();
        }

        [HttpGet("kaboom/4")]
        public ActionResult Kaboom4()
        {
            logger.LogError("Error");
            return Ok();
        }

        [HttpGet("kaboom/5")]
        public ActionResult Kaboom5()
        {
            logger.LogCritical("Critical");
            return Ok();
        }
    }
}
