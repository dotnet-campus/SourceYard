using System;
using System.Windows;
using TheLib;

namespace AppUsingSource
{
    class Program
    {
        static void Main(string[] args)
        {
            var money = new Money(12312);
            var v = (money, "123");

            Console.WriteLine(money.ToCapital());
            Console.ReadLine();
        }
    }
}
