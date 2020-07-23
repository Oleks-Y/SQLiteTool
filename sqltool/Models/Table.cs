using System.Collections.Generic;

namespace ConsoleApp1.Models
{
    public class Table : ITable
    {
        public string Name { get; set; }
        public List<IColumn> Columns { get; set; }
        public string[,] Data { get; set; }

        public Table()
        {
            Columns = new List<IColumn>();
        }
    }
}