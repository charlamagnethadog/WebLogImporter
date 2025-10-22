using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Fenton.WebLogImporter;

public interface IWebLogsFiles
{
    public DateTime MinDate { get ; }
    public DateTime MaxDate { get ; }
    OutFile Configure(string logFolder, string siteName);
    FileHeader ProcessFiles();
}

public class WebLogsFiles : IWebLogsFiles
{
    //private const string _in = "D:\\Research\\weblogs\\W3SVC2";
    //private const string _out = "D:\\Research\\weblogs\\out2";
    private const string _outFile = "D:\\Research\\weblogs\\out.log";
    private const string _comment = "#";
    private const string _header = "#Fields:";
    private const string _extra_header = " sitename";

    private string _in;
    private string _site;
    private DateTime _minDate;
    private DateTime _maxDate;

    public DateTime MinDate { get { return _minDate; } }
    public DateTime MaxDate { get { return _maxDate; } }

    public OutFile Configure(string logFolder, string siteName)
    {
        if (!Directory.Exists(logFolder))
            throw new ArgumentException($"folder {logFolder} does not exist", "logFolder");

        _in = logFolder;
        _site = siteName;

        //CreateDirectories();
        RemoveOldFiles();
        return new OutFile(_outFile);
    }

    public FileHeader ProcessFiles()
    {
        bool headerRowProcessed = false;
        string headerRow = string.Empty;

        using StreamWriter outfile = new StreamWriter(File.Create(_outFile));

        var files = Directory.GetFiles(_in);
        Console.WriteLine($"Found {files.Length} log files in '{_in}'");
        BigInteger logLines = 0;
        BigInteger commentLines = 0;
        _minDate = DateTime.MaxValue;
        _maxDate = DateTime.MinValue;
        foreach (string file in files)
        {
            Console.Write($"{file}\r");

            using FileStream fileStream = new FileStream(file, FileMode.Open);
            using StreamReader reader = new StreamReader(fileStream);

            string line = reader.ReadLine();

            while (line != null)
            {
                if (line.StartsWith(_comment))
                {
                    if (!headerRowProcessed && line.StartsWith(_header))
                    {
                        headerRowProcessed = true;
                        headerRow = line + _extra_header;
                        outfile.WriteLine(line + _extra_header);
                    }
                    commentLines++;
                }
                else
                {
                    var fields = line.Split(' ');
                    DateOnly logDate = DateOnly.Parse(fields[0]);
                    TimeOnly logTime = TimeOnly.Parse(fields[1]);
                    DateTime logDateTime = new DateTime(logDate, logTime);

                    if (logDateTime < _minDate ) _minDate = logDateTime;
                    if (logDateTime > _maxDate ) _maxDate = logDateTime;

                    outfile.WriteLine(line + " " + _site);
                    logLines++;
                }

                line = reader.ReadLine();
            }
        }

        outfile.Close();
        Console.WriteLine($"For site: {_site}; Log file count: {files.Length}; log line count: {logLines:N0}; comment line count: {commentLines:N0};");
        Console.WriteLine($"Earliest log entry: {_minDate}, Latest log entry: {_maxDate}");

        return new FileHeader(headerRow);
    }

    private static void RemoveOldFiles()
    {
        if (File.Exists(_outFile))
        {
            File.Delete(_outFile);
        }
    }

    //private void CreateDirectories()
    //{
    //    IList<string> directories = new List<string>
    //    {
    //        //"c:\\Temp",
    //        //"c:\\Temp\\Logs",
    //        //_in,
    //        //_out,
    //    };

    //    foreach (var directory in directories)
    //    {
    //        Directory.CreateDirectory(directory);
    //    }
    //}
}
