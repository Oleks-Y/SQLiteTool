using System;

namespace ConsoleApp1.SQLResults
{
    public abstract class SQLResult
    {
        public string Message { get; set; }
        public ConsoleColor ConsoleColor { get; set; }
        
        public virtual string[,] Data { get; set; }
        public SQLResult(string message)
        {
            Message = message;
        }
    }
}