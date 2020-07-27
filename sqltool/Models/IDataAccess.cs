using System.Collections.Generic;


namespace ConsoleApp1.Models
{
    public interface IDataAccess
    {
        void  ExecuteQuery(string query);

        string[,] ExecuteQueryResult(string query);

        void InsertQuery(Dictionary<string, string> data, string tableName);

        ITable GetColumnNames(string tableName);

        void NewQuery();
    }
}