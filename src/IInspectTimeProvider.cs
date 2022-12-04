namespace BorfoamInspection;

/// <summary>
/// Inspection record time value acquisition service
/// </summary>
public interface IInspectTimeProvider
{
    /// <summary>Get time value.</summary>
    /// <returns>Time value</returns>
    long GetTime();
}

/// <summary>
/// Inspection record time value acquisition service using DateTime current time
/// </summary>
public class DateTimeInspectTimeProvider : IInspectTimeProvider
{
    /// <inheritdoc />
    public long GetTime() => DateTime.Now.Ticks;
}