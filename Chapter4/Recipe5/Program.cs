using System;
using System.ComponentModel;
using System.Threading.Tasks;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter4.Recipe5
{
	class Program
	{
		static void Main(string[] args)
		{
			var tcs = new TaskCompletionSource<int>();

			var worker = new BackgroundWorker();
			worker.DoWork += (sender, eventArgs) =>
			{
				eventArgs.Result = TaskMethod("Background worker", 5);
			};

			worker.RunWorkerCompleted += (sender, eventArgs) =>
			{
				if (eventArgs.Error != null)
				{
					tcs.SetException(eventArgs.Error);
				}
				else if (eventArgs.Cancelled)
				{
					tcs.SetCanceled();
				}
				else
				{
					tcs.SetResult((int)eventArgs.Result);
				}
			};

			worker.RunWorkerAsync();

			int result = tcs.Task.Result;

			WriteLine($"Result is: {result}");
		}

		static int TaskMethod(string name, int seconds)
		{
			WriteLine(
			    $"Task {name} is running on a thread id " +
			    $"{CurrentThread.ManagedThreadId}. Is thread pool thread: " +
			    $"{CurrentThread.IsThreadPoolThread}");

			Sleep(TimeSpan.FromSeconds(seconds));
			return 42 * seconds;
		}
	}
}
