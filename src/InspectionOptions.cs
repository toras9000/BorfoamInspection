namespace BorfoamInspection;

/// <summary>
/// Inspection record options
/// </summary>
public class InspectionOptions
{
    /// <summary>Maximum number of logs</summary>
    public int LogCount { get; set; }

    /// <summary>Whether or not to obtain a stack trace.</summary>
    public bool CaptureStackTrace { get; set; }

    /// <summary>Whether or not file information is obtained when acquiring stack traces.</summary>
    public bool StackFileInfo { get; set; }

    /// <summary>Output directory at log dump.</summary>
    public DirectoryInfo? LogDir { get; set; }
}
