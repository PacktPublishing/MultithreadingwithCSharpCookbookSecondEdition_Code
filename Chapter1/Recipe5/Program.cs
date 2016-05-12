using System;
using System.Threading;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter1.Recipe5
{
	class Program
	{
		static void Main(string[] args)
		{
			WriteLine("Starting program...");
			Thread t = new Thread(PrintNumbersWithStatus);
			Thread t2 = new Thread(DoNothing);
			WriteLine(t.ThreadState.ToString());
			t2.Start();
			t.Start();
			for (int i = 1; i < 30; i++)
			{
				WriteLine(t.ThreadState.ToString());
			}
			Sleep(TimeSpan.FromSeconds(6));
			t.Abort();
			WriteLine("A thread has been aborted");
			WriteLine(t.ThreadState.ToString());
			WriteLine(t2.ThreadState.ToString());
		}

		static void DoNothing()
		{
			Sleep(TimeSpan.FromSeconds(2));
		}

		static void PrintNumbersWithStatus()
		{
			WriteLine("Starting...");
			WriteLine(CurrentThread.ThreadState.ToString());
			for (int i = 1; i < 10; i++)
			{
				Sleep(TimeSpan.FromSeconds(2));
				WriteLine(i);
			}
		}
	}
}
