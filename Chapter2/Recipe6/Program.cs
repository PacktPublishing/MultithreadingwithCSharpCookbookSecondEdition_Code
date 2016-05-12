using System;
using System.Threading;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter2.Recipe6
{
	class Program
	{
		static void Main(string[] args)
		{
			WriteLine("Starting two operations");
			var t1 = new Thread(() => PerformOperation("Operation 1 is completed", 4));
			var t2 = new Thread(() => PerformOperation("Operation 2 is completed", 8));
			t1.Start();
			t2.Start();
			_countdown.Wait();
			WriteLine("Both operations have been completed.");
			_countdown.Dispose();
		}

		static CountdownEvent _countdown = new CountdownEvent(2);

		static void PerformOperation(string message, int seconds)
		{
			Sleep(TimeSpan.FromSeconds(seconds));
			WriteLine(message);
			_countdown.Signal();
		}
	}
}
