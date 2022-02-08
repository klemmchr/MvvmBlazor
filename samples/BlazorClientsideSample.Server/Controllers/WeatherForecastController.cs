using System;
using System.Collections.Generic;
using System.Linq;
using BlazorSample.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BlazorClientsideSample.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet]
    public IEnumerable<WeatherForecastEntity> Get()
    {
        var rng = new Random();
        return Enumerable.Range(1, 5)
            .Select(
                index => new WeatherForecastEntity
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                }
            )
            .ToArray();
    }
}