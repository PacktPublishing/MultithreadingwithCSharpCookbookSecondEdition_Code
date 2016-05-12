using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter4.Recipe6
{
	class Program
	{
		static void Main(string[] args)
		{
			var cts = new CancellationTokenSource();
			var longTask = new Task<int>(() => TaskMethod("Task 1", 10, cts.Token), cts.Token);
			WriteLine(longTask.Status);
			cts.Cancel();
			WriteLine(longTask.Status);
			WriteLine("First task has been cancelled before execution");

			cts = new CancellationTokenSource();
			longTask = new Task<int>(() => TaskMethod("Task 2", 10, cts.Token), cts.Token);
			longTask.Start();
			for (int i = 0; i < 5; i++ )
			{
				Sleep(TimeSpan.FromSeconds(0.5));
				WriteLine(longTask.Status);
			}
			cts.Cancel();
			for (int i = 0; i < 5; i++)
			{
				Sleep(TimeSpan.FromSeconds(0.5));
				WriteLine(longTask.Status);
			}

			WriteLine($"A task has been completed with result {longTask.Result}.");
		}

static int TaskMethod(string name, int seconds, CancellationToken token)
{
	WriteLine(
		$"Task {name} is running on a thread id " +
		$"{CurrentThread.ManagedThreadId}. Is thread pool thread: " +
		$"{CurrentThread.IsThreadPoolThread}");

	for (int i = 0; i < seconds; i ++)
	{
		Sleep(TimeSpan.FromSeconds(1));
		if (token.IsCancellationRequested) return -1;
	}
	return 42*seconds;
}
	}
}