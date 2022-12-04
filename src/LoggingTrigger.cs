namespace BorfoamInspection;

/// <summary>
/// Trigger log save
/// </summary>
public class LoggingTrigger
{
    // Construction
    #region Constructor
    /// <summary>Construct an instance by specifying configuration information.</summary>
    /// <param name="threthold">Number of times the trigger condition is met before the save is executed.</param>
    /// <param name="recorder">Trigger target recorder</param>
    public LoggingTrigger(int threthold = 1, InspectionRecorder? recorder = null)
    {
        if (threthold <= 0) throw new ArgumentOutOfRangeException(nameof(threthold));

        this.Trethold = threthold;
        this.Recorder = recorder ?? InstanteInspector.Instance;
        this.spinner = new SpinLock();
    }
    #endregion

    // Public properties
    #region Instance Information
    /// <summary>Number of times the trigger condition is met before the save is executed.</summary>
    public int Trethold { get; }

    /// <summary>Trigger target recorder</summary>
    public InspectionRecorder Recorder { get; }
    #endregion

    // Public mthods
    #region Trigger
    /// <summary>Triggered by the time of the period.</summary>
    /// <param name="time">Trigger reference time.Triggers when the interval exceeds this time.</param>
    /// <returns>An object representing an period.Discarding this object terminates the period.</returns>
    public IDisposable TimePeriod(int time)
    {
        var begin = DateTime.Now;
        return new ReserveCondition(this, () => time <= (DateTime.Now - begin).Milliseconds);
    }

    /// <summary>Trigger by conditions.</summary>
    /// <param name="condition">Whether to trigger or not.</param>
    public void Condition(bool condition)
    {
        if (condition)
        {
            countTrigger();
        }
    }
    #endregion

    // Private types
    #region Internal process type
    /// <summary>
    /// Class to determine conditions at the time of discarding
    /// </summary>
    private sealed class ReserveCondition : IDisposable
    {
        public ReserveCondition(LoggingTrigger outer, Func<bool> eval)
        {
            this.trigger = outer ?? throw new ArgumentNullException(nameof(outer));
            this.evaluator = eval ?? throw new ArgumentNullException(nameof(eval));
        }

        public void Dispose()
        {
            if (this.trigger != null)
            {
                this.trigger.Condition(this.evaluator());
                this.trigger = null!;
                this.evaluator = null!;
            }
        }

        private LoggingTrigger trigger;
        private Func<bool> evaluator;
    }
    #endregion

    // Private fields
    #region Status
    /// <summary>Trigger counter</summary>
    private int counter;

    /// <summary>Counter Lock Object</summary>
    private SpinLock spinner;
    #endregion

    // Private methods
    #region Trigger
    /// <summary>Counting and triggering the fulfillment of conditions.</summary>
    private void countTrigger()
    {
        var lcoked = false;
        var triggered = false;
        try
        {
            this.spinner.Enter(ref lcoked);

            this.counter++;
            if (this.Trethold <= this.counter)
            {
                this.counter = 0;
                triggered = true;
            }
        }
        finally
        {
            if (lcoked) this.spinner.Exit();
        }

        if (triggered)
        {
            this.Recorder.Dump();
        }
    }
    #endregion
}
