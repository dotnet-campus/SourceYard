using System;
using TheLib;

namespace AppUsingDll
{
    class Program
    {
        static void Main(string[] args)
        {
            var money = new Money(12312);
            Console.WriteLine(money.ToCapital());
            Console.ReadLine();
        }
    }
}
