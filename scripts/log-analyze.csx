#r "nuget: Lestaly, 0.17.0"
using Lestaly;

record LogInfo(DateTime Time, int ThreadId, string Message, string Stack);

await Paved.RunAsync(async () =>
{
    var filePath = ConsoleWig.ReadLineDirect("Log path\n>");
    if (filePath.IsWhite()) return;

    var logFile = new FileInfo(filePath);

    var threads = new List<int>();
    var logs = logFile.ReadAllText().SplitFields()
        .Skip(1)
        .Select(fields =>
        {
            var time = new DateTime(fields[0].ParseInt64());
            var id = fields[1].ParseInt32();
            var msg = fields[2];
            var trace = fields[3].Decorate(t => Encoding.UTF8.GetString(Convert.FromBase64String(t)));
            if (!threads.Contains(id))
            {
                threads.Add(id);
            }
            return new LogInfo(time, id, msg, trace);
        })
        .ToArray();
    if (logs.Length <= 0) throw new PavedMessageException("No record");

    var procFile = logFile.Directory.GetRelativeFile($"{logFile.Name}.proc.csv");
    using var writer = procFile.CreateTextWriter();
    writer.WriteLine(threads.Select(n => "TID:" + n).Prepend("Time").JoinString(","));
    var line = new string[1 + threads.Count];
    var prevTime = logs[0].Time;
    foreach (var log in logs)
    {
        Array.Fill(line, "");

        var span = log.Time - prevTime;
        if (1000 < span.TotalMilliseconds)
        {
            line[0] = "<Gap>";
            writer.WriteLine(line.JoinString(","));
        }

        var idx = threads.IndexOf(log.ThreadId);
        line[0] = log.Time.ToString("yyyy/MM/dd HH:mm:ss.fff");
        line[1 + idx] = log.Message;
        writer.WriteLine(line.JoinString(","));
    }

});