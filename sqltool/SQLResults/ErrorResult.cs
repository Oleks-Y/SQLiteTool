using System;

namespace ConsoleApp1.SQLResults
{
    public class ErrorResult : SQLResult
    {
        public ErrorResult(string message) : base(message)
        {
        }
    }
}