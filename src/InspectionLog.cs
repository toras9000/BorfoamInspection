using System.Diagnostics;

namespace BorfoamInspection;

/// <summary>
/// Inspection data
/// </summary>
public class InspectionLog
{
    /// <summary>Record time</summary>
    public long Time { get; set; }

    /// <summary>Inspection message</summary>
    public string Message { get; set; } = "";

    /// <summary>Managed thread ID</summary>
    public int ThreadId { get; set; }

    /// <summary>Stack trace (if enabled)</summary>
    public StackTrace? Stack { get; set; }
}
