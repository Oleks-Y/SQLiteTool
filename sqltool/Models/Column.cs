﻿namespace ConsoleApp1.Models
{
    public class Column : IColumn
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool IsNull { get; set; }
        public bool IsPk { get; set; }
    }
}