namespace BorfoamInspection;

/// <summary>
/// Instantly available singleton inspector.
/// </summary>
public static class InstanteInspector
{
    /// <summary>Instant Inspector Settings.</summary>
    /// <remarks>Only settings before the singleton instance is created are valid.</remarks>
    public static InspectionOptions Options { get; } = new InspectionOptions();

    /// <summary>Singleton inspector instance.</summary>
    public static InspectionRecorder Instance => Generator.Value;

    // private
    #region internals
    /// <summary>Static constructor</summary>
    static InstanteInspector()
    {
        Generator = new(() => new InspectionRecorder(Options));
    }

    /// <summary>Delayed Instance Generation</summary>
    private static readonly Lazy<InspectionRecorder> Generator;
    #endregion
}
