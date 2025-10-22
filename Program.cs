using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fenton.WebLogImporter;

public static class Program
{
    public static void Main(string[] args)
    {
        string m_LogFolder;
        string m_SiteName;
        
        if (args.Length != 2)
            usage("Must specific (1) log folder, (2) site name");
        m_LogFolder = args[0];
        m_SiteName = args[1].TrimToLength(20);

        if (!Directory.Exists(m_LogFolder))
            usage($"Folder {m_LogFolder} does not exist");

        var logFiles = Directory.EnumerateFiles(m_LogFolder, "*.log");

        TimedOperation(() =>
        {
            IWebLogsFiles files = new WebLogsFiles();
            OutFile outFile = files.Configure(m_LogFolder, m_SiteName);
            FileHeader header = files.ProcessFiles();

            IWebLogsDatabase db = new WebLogsDatabase(header, outFile);
            db.Configure(true); //do not delete existing DB objects
            db.BulkLoadData(files.MinDate, files.MaxDate, m_SiteName);
        });

#if debug
        Console.ReadKey();
#endif
    }

    private static void TimedOperation(Action action)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();

        action();

        timer.Stop();
        Console.WriteLine($"Elapsed time {timer.ElapsedMilliseconds:N0} ms");
    }

    private static void usage(string message)
    {
        Console.WriteLine(message);
        Environment.Exit(1);
    }
}
