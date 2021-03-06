﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ConsoleApp1.Models;


namespace ConsoleApp1
{
    class Program
    {
        private static string connectionString;
        private static IDataAccess executor;
        static void Main(string[] args)
        {
            connectionString = ReadVariable();
            
            switch (args.Length)
            {
                case 4:
                    switch (args[0])
                    {
                        case "-psgr":
                            // DataBase Login Password
                            connectionString =
                                $"Host=localhost;Username={args[2]};Password={args[3]};Database={args[1]}";
                            executor = new PostgresDataAccess(connectionString);
                            
                            SaveValue(connectionString);
                            SaveType("postgres");
                            break;
                        default:
                            Console.Write("\n  Command not found check the option ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(@"--h");
                            Console.ResetColor();
                            Console.Write(" for more information \n\n");
                            break;
                    }
                    break;
                case 2 :
                    switch (args[0])
                    {
                        case "-sqlite":
                            SaveValue(args[1]);
                            SaveType("sqlite");
                            connectionString = args[1];
                            executor = new SqLiteDataAccess(connectionString);
                            
                            break;
                       
                        case "-q" when connectionString == "":
                            Console.Write("\n Please Specify the sqlite database path first by typing ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(@"-db ""path""" + Environment.NewLine + Environment.NewLine);
                            Console.ResetColor();
                            break;
                        case "-r" when connectionString == "":
                            Console.Write("\n Please Specify the sqlite database path first by typing ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(@"-db ""path""" + Environment.NewLine + Environment.NewLine);
                            Console.ResetColor();
                            break;
                        case "-q":
                            //Console.WriteLine("Debug.Executing query");
                            
                            ExecuteQuery(args[1]);
                            break;
                        case "-r":
                            //Console.WriteLine("Debug.Executing query with result");
                            
                            ExecuteQueryWithResult(args[1]);
                            break;
                        case "-addin":
                            AddValues(args[1]);
                            break;
                        default:
                            Console.Write("\n  Command not found check the option ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(@"--h");
                            Console.ResetColor();
                            Console.Write(" for more information \n\n");
                            break;
                    }
                    break;
                case 1 when args[0] == "--h":
                    Console.WriteLine("\n => Welcome to sqlite .net core global tool version 1.0");
                    Console.WriteLine("\nOptions:");
                    Console.WriteLine(@"   -sqlite ""Sqlite Database Path""");
                    Console.WriteLine(@"   -psgr database_name login password");
                    Console.WriteLine(@"   -q  ""Query to execute""");
                    Console.WriteLine(@"   -r  ""Query to execute with result""");
                    Console.WriteLine(@"   -s  ""the table that you want to show, its data""");
                    Console.WriteLine(@"   -addin  ""the table that you want to add data in """);
                    Console.WriteLine();
                    break;
                case 1:
                    Console.WriteLine("\n Need to insert a value for the options");
                    break;
                default : 
                    Console.Write("\n   Check the help section by typing ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("sqlite-tool --h \n");
                    Console.ResetColor();
                    break;
            }
        }
        private static void ExecuteQuery(string query)
        {
            executor = CreateConnection();
            if (connectionString == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Specify table!");
                Console.ResetColor();
            }
            try
            {
                executor.ExecuteQuery(query);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Query executed successfully");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        private static void ExecuteQueryWithResult(string query)
        {
            executor = CreateConnection();
            if (connectionString == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Specify table!");
                Console.ResetColor();
            }
            var result = executor.ExecuteQueryResult(query);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Query executed successfully");
            Console.ResetColor();
            try
            {
                Console.WriteLine(TableMaker.GetDataInTableFormat(result));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }

        }
       
        private static void AddValues(string tableName)
        {
            executor = CreateConnection();
            // Show form with all columns to fill the data
            ITable colums  =new Table();
            try
            {
                colums = executor.GetColumnNames(tableName);
            }
            catch (NullDbResultException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid query");
                Console.ResetColor();
            }

            var input = new Dictionary<string, string>();
            foreach (var column in colums.Columns)
            {
                Console.Write(column.Name);
                Console.Write($"({column.DataType}):");
                string value = "";
                
                while ((!column.IsNull && value==""))
                {
                    value = Console.ReadLine();
                }
                input.Add(column.Name, value);
            }
            executor.NewQuery();
            executor.InsertQuery(input, tableName);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Query executed successfully");
            Console.ResetColor();
        }

        private static IDataAccess CreateConnection()
        {
            string type = ReadType();
            switch (type)
            {
                case "sqlite":
                    return new SqLiteDataAccess(connectionString);
                    break;
                case "postgres":
                    return new PostgresDataAccess(connectionString);
                    break;
                default:
                    return null;
            }
            
        }
        
        
        #region Cashe
        private static string ReadVariable()
        {
            const string path = "SampleNew.txt";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                using (TextWriter tw = new StreamWriter(path))
                {
                    tw.Write("empty");
                }
            }
            var sr = new StreamReader(path);
            var line = sr.ReadLine();
            sr.Close();
            return line;
        }
        private static string ReadType()
        {
            const string path = "Type.txt";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                using (TextWriter tw = new StreamWriter(path))
                {
                    tw.Write("empty");
                }
            }
            var sr = new StreamReader(path);
            var line = sr.ReadLine();
            sr.Close();
            return line;
        }

        

        private static void SaveValue(string value)
        {
            var sw = new StreamWriter("SampleNew.txt");
            
            sw.Write(value);
            sw.Close();
            

            Console.WriteLine("\n Database saved successfully \n");
        }
        private static void SaveType(string value)
        {
            var sw = new StreamWriter("Type.txt");
            
            sw.Write(value);
            sw.Close();
            

            Console.WriteLine("\n Database saved successfully \n");
        }
        #endregion
   }
    
    
   
   
   //Todo write documentation on github
   //TODO Add more functionality
   
  
    
    
}