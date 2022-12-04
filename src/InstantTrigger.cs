namespace BorfoamInspection;

/// <summary>
/// Instantly available singleton trigger.
/// </summary>
public static class InstantTrigger
{
    /// <summary>Number of times the trigger condition is met before the save is executed.</summary>
    /// <remarks>Only settings before the singleton instance is created are valid.</remarks>
    public static int Threshold { get; set; }

    /// <summary>Singleton trigger instance</summary>
    public static LoggingTrigger Instance => Generator.Value;

    // private
    #region internals
    /// <summary>Static constructor</summary>
    static InstantTrigger()
    {
        Generator = new(() => new LoggingTrigger(Threshold));
    }

    /// <summary>Delayed Instance Generation</summary>
    private static readonly Lazy<LoggingTrigger> Generator;
    #endregion
}
