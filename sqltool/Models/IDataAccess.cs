using System.Collections.Generic;
using ConsoleApp1.SQLResults;

namespace ConsoleApp1.Models
{
    public interface IDataAccess
    {
        SQLResult  ExecuteQuery(string query);

        SQLResult ExecuteQueryResult(string query);

        SQLResult InsertQuery(Dictionary<string, string> data, string tableName);

        ITable GetColumnNames(string tableName);

        void NewQuery();
    }
}