using System.Globalization;

namespace Obstor.UnitTests.Services;

public class StaticTimeProvider : ITimeProvider
{
    public StaticTimeProvider(string time)
    {
        UtcNow = DateTime.ParseExact(time, "yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture).ToUniversalTime();
    }
    
    public DateTime UtcNow { get; }
}