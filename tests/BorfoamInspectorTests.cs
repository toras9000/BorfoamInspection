using System.Diagnostics;
using FluentAssertions;

namespace BorfoamInspection.Tests;

[TestClass]
public class BorfoamInspectorTests
{
    [TestMethod]
    public async Task Dump()
    {
        InstanteInspector.Options.LogCount = 10000;
        InstanteInspector.Options.CaptureStackTrace = true;
        InstanteInspector.Options.StackFileInfo = true;

        var randomer = new Random();
        var tasks = Enumerable.Range(0, 30)
            .Select(n =>
            {
                var time1 = randomer.Next(3000);
                var time2 = randomer.Next(3000);
                return Task.Run(async () =>
                {
                    InstanteInspector.Instance.Record($"In");
                    await Task.Delay(time1);
                    InstanteInspector.Instance.Record($"Middle");
                    await Task.Delay(time2);
                    InstanteInspector.Instance.Record($"Out");
                });
            })
            .ToArray();

        await Task.WhenAll(tasks);

        InstanteInspector.Instance.Dump();

        Process.Start(new ProcessStartInfo
        {
            FileName = InstanteInspector.Instance.Output.FullName,
            UseShellExecute = true,
        });
    }

    [TestMethod]
    public async Task Trigger()
    {
        var recorder = new InspectionRecorder();
        var trigger = new LoggingTrigger(threthold: 3, recorder);

        for (var i = 0; i < 10; i++)
        {
            using var p = trigger.TimePeriod(300);
            await Task.Delay(50 + i * 100);
            recorder.Record($"[{i}] trigger");
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = recorder.Output.FullName,
            UseShellExecute = true,
        });

    }
}