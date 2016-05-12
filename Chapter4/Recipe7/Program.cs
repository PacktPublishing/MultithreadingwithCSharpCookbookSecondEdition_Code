using System;
using System.Threading.Tasks;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter4.Recipe7
{
	class Program
	{
		static void Main(string[] args)
		{
			Task<int> task;
			try
			{
				task = Task.Run(() => TaskMethod("Task 1", 2));
				int result = task.Result;
				WriteLine($"Result: {result}");
			}
			catch (Exception ex)
			{
				WriteLine($"Exception caught: {ex}");
			}
			WriteLine("----------------------------------------------");
			WriteLine();

			try
			{
				task = Task.Run(() => TaskMethod("Task 2", 2));
				int result = task.GetAwaiter().GetResult();
                WriteLine($"Result: {result}");
            }
			catch (Exception ex)
			{
				WriteLine($"Exception caught: {ex}");
			}
			WriteLine("----------------------------------------------");
			WriteLine();

			var t1 = new Task<int>(() => TaskMethod("Task 3", 3));
			var t2 = new Task<int>(() => TaskMethod("Task 4", 2));
			var complexTask = Task.WhenAll(t1, t2);
			var exceptionHandler = complexTask.ContinueWith(t => 
					WriteLine($"Exception caught: {t.Exception}"), 
					TaskContinuationOptions.OnlyOnFaulted
				);
			t1.Start();
			t2.Start();

			Sleep(TimeSpan.FromSeconds(5));
		}

		static int TaskMethod(string name, int seconds)
		{
			WriteLine(
			    $"Task {name} is running on a thread id " +
			    $"{CurrentThread.ManagedThreadId}. Is thread pool thread: " +
			    $"{CurrentThread.IsThreadPoolThread}");

			Sleep(TimeSpan.FromSeconds(seconds));
			throw new Exception("Boom!");
			return 42 * seconds;
		}
	}
}
