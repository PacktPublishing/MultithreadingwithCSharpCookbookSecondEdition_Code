using System;
using System.Threading;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter2.Recipe5
{
	class Program
	{
		static void Main(string[] args)
		{
			var t1 = new Thread(() => TravelThroughGates("Thread 1", 5));
			var t2 = new Thread(() => TravelThroughGates("Thread 2", 6));
			var t3 = new Thread(() => TravelThroughGates("Thread 3", 12));
			t1.Start();
			t2.Start();
			t3.Start();
			Sleep(TimeSpan.FromSeconds(6));
			WriteLine("The gates are now open!");
			_mainEvent.Set();
			Sleep(TimeSpan.FromSeconds(2));
			_mainEvent.Reset();
			WriteLine("The gates have been closed!");
			Sleep(TimeSpan.FromSeconds(10));
			WriteLine("The gates are now open for the second time!");
			_mainEvent.Set();
			Sleep(TimeSpan.FromSeconds(2));
			WriteLine("The gates have been closed!");
			_mainEvent.Reset();
		}

		static void TravelThroughGates(string threadName, int seconds)
		{
			WriteLine($"{threadName} falls to sleep");
			Sleep(TimeSpan.FromSeconds(seconds));
			WriteLine($"{threadName} waits for the gates to open!");
			_mainEvent.Wait();
			WriteLine($"{threadName} enters the gates!");
		}

		static ManualResetEventSlim _mainEvent = new ManualResetEventSlim(false);
	}
}
