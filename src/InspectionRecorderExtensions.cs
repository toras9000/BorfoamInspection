using System.Runtime.CompilerServices;

namespace BorfoamInspection;

/// <summary>
/// Extension method of inspection recorder.
/// </summary>
public static class InspectionRecorderExtensions
{
    /// <summary>Record in a message with the caller method information.</summary>
    /// <param name="self">Target Instance</param>
    /// <param name="message">Record message</param>
    /// <param name="over">Number of relay methods.</param>
    /// <param name="capture">Override the setting for getting a stack trace.</param>
    /// <param name="member">Calling member</param>
    /// <param name="file">Calling file</param>
    /// <param name="line">Calling line number</param>
    public static void RecordMember(this InspectionRecorder self, string message, int over = 0, bool? capture = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0)
    {
        self.Record($"{file}[{line}] : {member} - {message}", 1 + over, capture);
    }

    /// <summary>Only the position is recorded.</summary>
    /// <param name="self">Target Instance</param>
    /// <param name="over">Number of relay methods.</param>
    /// <param name="capture">Override the setting for getting a stack trace.</param>
    /// <param name="member">Calling member</param>
    /// <param name="file">Calling file</param>
    /// <param name="line">Calling line number</param>
    public static void RecordPosition(this InspectionRecorder self, int over = 0, bool? capture = null, [CallerMemberName] string? member = null, [CallerFilePath] string? file = null, [CallerLineNumber] int line = 0)
    {
        self.Record($"{file}[{line}] : {member}", 1 + over, capture);
    }
}
