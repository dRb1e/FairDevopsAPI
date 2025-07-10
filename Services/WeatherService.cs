using FairDevopsAPI.Models;

namespace FairDevopsAPI.Services;

public class WeatherService
{
    private readonly List<WeatherForecast> _forecasts;
    private int _nextId = 1;

    public WeatherService()
    {
        _forecasts = new List<WeatherForecast>
        {
            new WeatherForecast { Id = _nextId++, Date = DateTime.Now.AddDays(1), TemperatureC = 20, Summary = "Güneşli" },
            new WeatherForecast { Id = _nextId++, Date = DateTime.Now.AddDays(2), TemperatureC = 15, Summary = "Bulutlu" },
            new WeatherForecast { Id = _nextId++, Date = DateTime.Now.AddDays(3), TemperatureC = 25, Summary = "Sıcak" }
        };
    }

    public IEnumerable<WeatherForecast> GetAll()
    {
        return _forecasts;
    }

    public WeatherForecast? GetById(int id)
    {
        return _forecasts.FirstOrDefault(f => f.Id == id);
    }

    public WeatherForecast Create(WeatherForecast forecast)
    {
        forecast.Id = _nextId++;
        _forecasts.Add(forecast);
        return forecast;
    }

    public WeatherForecast? Update(int id, WeatherForecast forecast)
    {
        var existing = _forecasts.FirstOrDefault(f => f.Id == id);
        if (existing == null)
            return null;

        existing.Date = forecast.Date;
        existing.TemperatureC = forecast.TemperatureC;
        existing.Summary = forecast.Summary;

        return existing;
    }

    public bool Delete(int id)
    {
        var forecast = _forecasts.FirstOrDefault(f => f.Id == id);
        if (forecast == null)
            return false;

        _forecasts.Remove(forecast);
        return true;
    }
} 