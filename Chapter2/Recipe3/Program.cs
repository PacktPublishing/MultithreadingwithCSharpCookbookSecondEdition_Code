using System;
using System.Threading;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter2.Recipe3
{
	class Program
	{
		static void Main(string[] args)
		{
			for (int i = 1; i <= 6; i++)
			{
				string threadName = "Thread " + i;
				int secondsToWait = 2 + 2 * i;
				var t = new Thread(() => AccessDatabase(threadName, secondsToWait));
				t.Start();
			}
		}

		static SemaphoreSlim _semaphore = new SemaphoreSlim(4);

		static void AccessDatabase(string name, int seconds)
		{
			WriteLine($"{name} waits to access a database");
			_semaphore.Wait();
			WriteLine($"{name} was granted an access to a database");
			Sleep(TimeSpan.FromSeconds(seconds));
			WriteLine($"{name} is completed");
			_semaphore.Release();
		}
	}
}
