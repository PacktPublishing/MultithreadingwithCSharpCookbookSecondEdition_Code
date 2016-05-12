using System;
using System.Threading;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter1.Recipe4
{
	class Program
	{
		static void Main(string[] args)
		{
			WriteLine("Starting program...");
			Thread t = new Thread(PrintNumbersWithDelay);
			t.Start();
			Sleep(TimeSpan.FromSeconds(6));
			t.Abort();
			WriteLine("A thread has been aborted");
		}

		static void PrintNumbersWithDelay()
		{
			WriteLine("Starting...");
			for (int i = 1; i < 10; i++)
			{
				Sleep(TimeSpan.FromSeconds(2));
				WriteLine(i);
			}
		}
	}
}
