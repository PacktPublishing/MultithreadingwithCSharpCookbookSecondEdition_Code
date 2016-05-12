using System;
using System.Threading;
using static System.Console;

namespace Chapter1.Recipe1
{
	class Program
	{
		static void Main(string[] args)
		{
			Thread t = new Thread(PrintNumbers);
			t.Start();
			PrintNumbers();
		}

		static void PrintNumbers()
		{
			WriteLine("Starting...");
			for (int i = 1; i < 10; i++)
			{
				WriteLine(i);
			}
		}
	}
}
