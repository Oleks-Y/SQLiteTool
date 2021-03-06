﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using ConsoleApp1.Models;

using Microsoft.VisualBasic;

namespace ConsoleApp1
{
    public class SqLiteDataAccess : IDataAccess
    {
        private SQLiteConnection _connection;
        public static string DbPath { get; set; }

        public SqLiteDataAccess(string dbPath)
        {
            DbPath = dbPath;
            var connectionString = new SQLiteConnectionStringBuilder {DataSource = dbPath};
            _connection = new SQLiteConnection(connectionString.ConnectionString);
        }

        public void NewQuery()
        {
            var connectionString = new SQLiteConnectionStringBuilder {DataSource = DbPath};
            _connection = new SQLiteConnection(connectionString.ConnectionString); 
            
        }
        public void InsertQuery(Dictionary<string, string> data, string tableName)
        {
            // Check if columns in Dictionary matches column in table
            // OR Pass ITable element
            string query = 
                $"INSERT INTO {tableName} VALUES('{String.Join("','",data.Values.ToArray())}')";

            ExecuteQuery(query);
        }

        
        public void ExecuteQuery(string query)
        {
            using (_connection)
            {
                    _connection.Open();

                    using (var transation = _connection.BeginTransaction())
                    {
                        var cmd = _connection.CreateCommand();
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                        transation.Commit();
                    }
            }
        }

       

        public string[,] ExecuteQueryResult(string query)
        {
           
            
                using (_connection)
                {
                    _connection.Open();
                    var command = _connection.CreateCommand();
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
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

        public ITable GetColumnNames(string tableName)
        {
            string[,] result = ExecuteQueryResult($"SELECT * FROM pragma_table_info('{tableName}')");
            if (result == null)
            {
                throw new NullDbResultException();
            }
            Table table = new Table();
            for (int i = 1; i < result.GetLength(0); i++)
            {
                table.Columns.Add(new Column()
                {
                    Name = result[i, 1],
                    IsNull = (result[i, 3]=="1"),
                    DataType = result[i,2]
                });
            }
            return table;
        }

        
    }
}