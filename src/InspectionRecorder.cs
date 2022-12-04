using System.Diagnostics;
using System.Text;

namespace BorfoamInspection;

/// <summary>
/// Inspection Recorder
/// </summary>
public class InspectionRecorder
{
    // Construction
    #region Constructor
    /// <summary>Construct an instance by specifying configuration information.</summary>
    /// <param name="options">Inspection options. If omitted, 10000 logs without stack acquisition.</param>
    /// <param name="provider">Time value provider. If omitted, the time is obtained by DateTime.</param>
    public InspectionRecorder(InspectionOptions? options = null, IInspectTimeProvider? provider = null)
    {
        if (options == null)
        {
            this.MaxLogCount = 10000;
            this.stackCapture = false;
        }
        else
        {
            if (options.LogCount < 0) throw new ArgumentOutOfRangeException($"{nameof(options)}.{nameof(options.LogCount)}");
            this.MaxLogCount = options.LogCount == 0 ? 10000 : options.LogCount;
            this.stackCapture = options.CaptureStackTrace;
            this.stackFileInfo = options.StackFileInfo;
        }

        this.logs = new Queue<InspectionLog>(this.MaxLogCount);
        this.timeProvider = provider ?? new DateTimeInspectTimeProvider();
        this.Output = options?.LogDir ?? new DirectoryInfo(Path.Combine(Path.GetTempPath(), $"BorfoamInspection_{DateTime.Now:yyyyMMdd_HHmmss_fff}"));
    }
    #endregion

    // Public property
    #region Instance Information
    /// <summary>Maximum number of logs.</summary>
    private int MaxLogCount { get; }

    /// <summary>Output directory of log dump</summary>
    public DirectoryInfo Output { get; }
    #endregion

    // Public method
    #region Inspection
    /// <summary>Record the inspection</summary>
    /// <param name="message">Record message.</param>
    /// <param name="over">
    /// For when stack trace acquisition is enabled.
    /// How many intermediate calls were passed before this method call.
    /// The specified number of stacked frames are excluded.
    /// </param>
    /// <param name="capture">
    /// Override the setting for getting a stack trace.
    /// </param>
    public void Record(string message, int over = 0, bool? capture = null)
    {
        var threadId = Thread.CurrentThread.ManagedThreadId;
        var stack = default(StackTrace);
        if (capture ?? this.stackCapture)
        {
            stack = new StackTrace(1 + over, this.stackFileInfo);
        }
        lock (this.sync)
        {
            var log = new InspectionLog();
            log.Time = this.timeProvider.GetTime();
            log.Message = message ?? "";
            log.ThreadId = threadId;
            log.Stack = stack;

            if (this.MaxLogCount <= this.logs.Count)
            {
                this.logs.Dequeue();
            }
            this.logs.Enqueue(log);
        }
    }

    /// <summary>Clear the log.</summary>
    public void Clear()
    {
        lock (this.sync)
        {
            this.logs.Clear();
        }
    }

    /// <summary>Save the log.</summary>
    /// <remarks>
    /// Save the logs currently held in the output directory.
    /// The log is cleared when saved.
    /// </remarks>
    public void Dump()
    {
        var hist = default(InspectionLog[]);
        lock (this.sync)
        {
            hist = this.logs.ToArray();
            this.logs.Clear();
        }

        var time = DateTime.Now;
        Task.Run(() =>
        {
            this.Output.Create();
            var logName = $"log_{time:yyyyMMdd_HHmmss_fff}.log";
            var logPath = Path.Combine(this.Output.FullName, logName);
            var logLines = (hist ?? Array.Empty<InspectionLog>())
                .Select(log =>
                {
                    var escMsg = log.Message.Replace("\r", "<CR>").Replace("\n", "<LF>").Replace("\"", "\"\"");
                    var stack = default(string);
                    if (log.Stack != null)
                    {
                        stack = Convert.ToBase64String(Encoding.UTF8.GetBytes(log.Stack.ToString()));
                    }
                    return $"{log.Time},{log.ThreadId},\"{escMsg}\",{stack}";
                })
                .Prepend("Time,ThreadId,Message,Stack")
                ;
            File.WriteAllLines(logPath, logLines);
        });
    }
    #endregion

    // Private fields
    #region for control
    /// <summary>For exclusive use of log operations</summary>
    private readonly object sync = new object();
    #endregion

    #region for inspection
    /// <summary>Recorded logs</summary>
    private readonly Queue<InspectionLog> logs;

    /// <summary>Whether or not to obtain a stack trace.</summary>
    private readonly bool stackCapture;

    /// <summary>Whether or not file information is obtained when acquiring stack traces.</summary>
    private readonly bool stackFileInfo;

    /// <summary>Time value acquisition service</summary>
    private readonly IInspectTimeProvider timeProvider;
    #endregion
}
