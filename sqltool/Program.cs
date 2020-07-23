using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ConsoleApp1.Models;
using ConsoleApp1.SQLResults;

namespace ConsoleApp1
{
    class Program
    {
        private static string _dbPath;
        private static DataAccess executor;
        static void Main(string[] args)
        {
            _dbPath = ReadVariable();
            executor = new DataAccess(_dbPath);
            switch (args.Length)
            {
                case 2 :
                    switch (args[0])
                    {
                        case "-db":
                            SaveValue(args[1]);
                            _dbPath = args[1];
                            executor = new DataAccess(_dbPath);
                            break;
                        case "-q" when _dbPath == "":
                            Console.Write("\n Please Specify the sqlite database path first by typing ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(@"-db ""path""" + Environment.NewLine + Environment.NewLine);
                            Console.ResetColor();
                            break;
                        case "-r" when _dbPath == "":
                            Console.Write("\n Please Specify the sqlite database path first by typing ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(@"-db ""path""" + Environment.NewLine + Environment.NewLine);
                            Console.ResetColor();
                            break;
                        case "-q":
                            //Console.WriteLine("Debug.Executing query");
                            //TODO Execute query
                            ExecuteQuery(args[1]);
                            break;
                        case "-r":
                            //Console.WriteLine("Debug.Executing query with result");
                            //TODO Execute query with result
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
                    Console.WriteLine(@"   -db ""Sqlite Database Path""");
                    Console.WriteLine(@"   -q  ""Query to execute""");
                    Console.WriteLine(@"   -r  ""Query to execute with result""");
                    Console.WriteLine(@"   -s  ""the table that you want to show, its data""");
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
            if (_dbPath == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Specify table!");
                Console.ResetColor();
            }
            try
            {
                var result = executor.ExecuteQuery(query);
                Console.ForegroundColor = result.ConsoleColor;
                Console.WriteLine(result.Message);
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
            if (_dbPath == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Specify table!");
                Console.ResetColor();
            }
            var result = executor.ExecuteQueryResult(query);
            Console.ForegroundColor = result.ConsoleColor;
            Console.WriteLine(result.Message);
            Console.ResetColor();
            try
            {
                Console.WriteLine(TableMaker.GetDataInTableFormat(result.Data));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Operation don`t have output");
                Console.ResetColor();
            }

        }
        private static string ReadVariable()
        {
            const string path = "Sample.txt";
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
            var sw = new StreamWriter("Sample.txt");
            sw.Write(value);
            sw.Close();
            Console.WriteLine("\n Database saved successfully \n");
        }

        private static void AddValues(string tableName)
        {
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
            var response = executor.InsertQuery(input, tableName);
            Console.ForegroundColor = response.ConsoleColor;
            Console.WriteLine(response.Message);
            Console.ResetColor();
        } 

   }
    
   //TODO Check db path 
   //TODO Test all queries
   //TODO Add DI
   //TODO Add Simple Insert with form
   
    
    
}