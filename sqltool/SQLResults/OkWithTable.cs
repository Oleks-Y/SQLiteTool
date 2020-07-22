using System;

namespace ConsoleApp1.SQLResults
{
    public class OkWithTable: SQLResult
    {
        // TODO Rewrite all with interfaces 
        public override string[,] Data{ get; set; }
        
        public OkWithTable(string message, string[,] table) : base(message)
        {
            Data= table;
            this.ConsoleColor = ConsoleColor.Green;

        }
        
    }
}