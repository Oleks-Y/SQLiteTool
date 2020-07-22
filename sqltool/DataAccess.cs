﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using ConsoleApp1.SQLResults;
using Microsoft.VisualBasic;

namespace ConsoleApp1
{
    public class DataAccess
    {
        private readonly SQLiteConnection _connection;
        public static string DbPath { get; set; }

        public DataAccess(string dbPath)
        {
            DbPath = dbPath;
            var connectionString = new SQLiteConnectionStringBuilder {DataSource = dbPath};
            _connection = new SQLiteConnection(connectionString.ConnectionString);
        }

        public SQLResult ExecuteQuery(string query)
        {
            try
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

                return new OkResult("Query executed succesfuly");
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex.Message);
            }

        }

        public SQLResult ExecuteQueryResult(string query)
        {
            
            try
            {
                using (_connection)
                {
                    _connection.Open();
                    var command = _connection.CreateCommand();
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        // TODO Write some more effective shit 
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
                        return new OkWithTable($"Query succesfull : {query}", arrValues);


                    }
                }
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex.Message);
            }
        }

        
    }
}