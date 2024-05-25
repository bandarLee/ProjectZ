// IPrecipitationManager.cs
namespace DigitalRuby.WeatherMaker
{
    public interface IPrecipitationManager : IWeatherMakerManager
    {
        float RainIntensity { get; }
        float SnowIntensity { get; }
        float HailIntensity { get; }
        float SleetIntensity { get; }
        float CustomIntensity { get; }
    }
}
