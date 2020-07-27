using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp1.Models;

using Npgsql;

namespace ConsoleApp1
{
    public class PostgresDataAccess : IDataAccess
    {
        private NpgsqlConnection _connection;
        public static string ConnectionString { get; set; }

        public PostgresDataAccess(string cs)
        {
            
            //TODO Check if database exists!!!!!
         _connection = new NpgsqlConnection(cs);
         ConnectionString = cs;
        }
        public void ExecuteQuery(string query)
        {
            using (var cmd = new NpgsqlCommand(query, _connection))
            {
                    _connection.Open();

                    var result = cmd.ExecuteScalar();
            }
        }

        public string[,] ExecuteQueryResult(string query)
        {
            using (var cmd = new NpgsqlCommand(query, _connection))
            {
                _connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    int count = reader.FieldCount;
                    int numberRecord = 0;
                    //Load data from db to dictionary in format
                    // "Key"(name of column) : List<string>(values of row)
                    var values = new Dictionary<string, List<string>>();
                    for (var k = 0; k < count; k++)
                    {
                        //Initializing lists
                        var key = reader.GetName(k);
                        if (!(values.ContainsKey(key))) values.Add(key, new List<string>());
                    }
                        
                    int rows = 0;
                    while (reader.Read())
                    {    // Push data to thr lists 
                        foreach (var key in values.Keys)
                        {
                            values[key].Add((string) reader[key].ToString());
                        }
                        rows++;
                    }
                        
                    // Packing data in array string[,]
                    var arrValues = new string[rows+1,count];
                    for (var i = 0; i < count; i++)
                    {
                        var key = values.Keys.ToArray()[i]; 
                        arrValues[0, i] = " " +key + " ";
                        for (var j = 1; j < (rows+1); j++)
                        {
                            arrValues[j, i] = " " + values[key][j-1] + " ";
                        }
                    }

                    return arrValues;
                }
            }
        }

        public void InsertQuery(Dictionary<string, string> data, string tableName)
        {
            string query = 
                $"INSERT INTO {tableName} VALUES('{String.Join("','",data.Values.ToArray())}')";

            
        }

        public ITable GetColumnNames(string tableName)
        {
            string query = $"SELECT column_name, is_nullable, data_type  FROM information_schema.columns WHERE table_name   = '{tableName}';";
            string[,] result = ExecuteQueryResult(query);
            if (result == null)
            {
                throw  new NullDbResultException();
            }

            Table table = new Table();
            for (int i = 1; i < result.GetLength(0); i++)
            {
                table.Columns.Add(new Column()
                {
                    Name = result[i, 0],
                    IsNull = (result[i, 1]=="1"),
                    DataType = result[i,2]
                });
            }
            return table;
        }

        public void NewQuery()
        {
            _connection = new NpgsqlConnection(ConnectionString);
        }
    }
}