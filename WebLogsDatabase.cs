using System;
using System.Data.SqlClient;

namespace Fenton.WebLogImporter;

public interface IWebLogsDatabase
{
    void BulkLoadData(DateTime minDate, DateTime maxDate, string sitenam);
    void Configure(bool keepObjects);
}

public class WebLogsDatabase : IWebLogsDatabase
{
    private readonly string _db = "Server=.;Database=WebLogs;Trusted_Connection=True;MultipleActiveResultSets=true;Pooling=true";
    private readonly WebLogsSqlCommands _commands;

    //private bool tablesExist;
    private bool clearExistingRecords;

    public WebLogsDatabase(FileHeader header, OutFile file)
    {
        _commands = new WebLogsSqlCommands(header, file);
    }

    public void Configure(bool keepObjects)
    {
        clearExistingRecords = false;
        //tablesExist = CheckTableExists();
        if (CheckTableExists() && keepObjects)
        {
            clearExistingRecords = true;
            return;
        }
        DropObjects();
        CreateLogEntryTable();
        CreateObjects();
    }

    public void BulkLoadData(DateTime minDate, DateTime maxDate, string sitename)
    {
        Int32 rows;
        if (clearExistingRecords)
        {
            Console.WriteLine($"Removing records for site {sitename} between {minDate} and {maxDate}");
            rows = ClearData(minDate, maxDate, sitename);
            Console.WriteLine($"Deleted {rows:N0} rows from site {sitename}");
        }

        Console.Write("Loading database rows...\r");
        rows = LoadData();
        Console.WriteLine($"Loaded {rows:N0} rows to site {sitename}"); //TODO: calculate time taken
    }

    private int ClearData(DateTime minDate, DateTime maxDate, string sitename)
    {
        using var connection = new SqlConnection(_db);
        using SqlCommand command = _commands.GetClearRowsCommand(connection, minDate, maxDate, sitename);
        connection.Open();
        command.CommandTimeout = 3600; // 3600 seconds = 1 hour
        var rows = command.ExecuteNonQuery();
        return (rows);
    }

    private int LoadData()
    {
        using var connection = new SqlConnection(_db);
        using SqlCommand command = _commands.GetBulkInsertCommand(connection);
        connection.Open();
        command.CommandTimeout = 3600; // 3600 seconds = 1 hour
        var rows = command.ExecuteNonQuery();
        return (rows);
    }

    private bool CheckTableExists()
    {
        using var connection = new SqlConnection(_db);
        using SqlCommand command = _commands.CheckTableExists(connection);
        connection.Open();
        var rows = (Int32)command.ExecuteScalar();
        return (rows > 0);
    }

    private void CreateObjects()
    {
        using var connection = new SqlConnection(_db);
        using SqlCommand command = _commands.GetCreateObjectsCommand(connection);
        connection.Open();
        command.ExecuteNonQuery();
    }

    private void CreateLogEntryTable()
    {
        using var connection = new SqlConnection(_db);
        using SqlCommand command = _commands.GetTableCommand(connection);
        connection.Open();
        command.ExecuteNonQuery();
    }

    private void DropObjects()
    {
        using var connection = new SqlConnection(_db);
        using SqlCommand command = _commands.GetDropObjectsCommand(connection);
        connection.Open();
        command.ExecuteNonQuery();
    }
}
