using System;
using System.Threading.Tasks;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter4.Recipe3
{
	class Program
	{
		static void Main(string[] args)
		{
			var firstTask = new Task<int>(() => TaskMethod("First Task", 3));
			var secondTask = new Task<int>(() => TaskMethod("Second Task", 2));

			firstTask.ContinueWith(
				t => WriteLine(
				    $"The first answer is {t.Result}. Thread id " +
				    $"{CurrentThread.ManagedThreadId}, is thread pool thread: " +
				    $"{CurrentThread.IsThreadPoolThread}"),
				TaskContinuationOptions.OnlyOnRanToCompletion);

			firstTask.Start();
			secondTask.Start();

			Sleep(TimeSpan.FromSeconds(4));

			Task continuation = secondTask.ContinueWith(
				t => WriteLine(
				    $"The second answer is {t.Result}. Thread id " +
				    $"{CurrentThread.ManagedThreadId}, is thread pool thread: " +
				    $"{CurrentThread.IsThreadPoolThread}"),
				TaskContinuationOptions.OnlyOnRanToCompletion 
                | TaskContinuationOptions.ExecuteSynchronously);

			continuation.GetAwaiter().OnCompleted(
				() => WriteLine(
				    $"Continuation Task Completed! Thread id " +
				    $"{CurrentThread.ManagedThreadId}, is thread pool thread: " +
				    $"{CurrentThread.IsThreadPoolThread}"));

			Sleep(TimeSpan.FromSeconds(2));
			WriteLine();

			firstTask = new Task<int>(() =>
			{
				var innerTask = Task.Factory.StartNew(() => TaskMethod("Second Task", 5),
                    TaskCreationOptions.AttachedToParent);

				innerTask.ContinueWith(t => TaskMethod("Third Task", 2),
                    TaskContinuationOptions.AttachedToParent);

				return TaskMethod("First Task", 2);
			});

			firstTask.Start();

			while (!firstTask.IsCompleted)
			{
				WriteLine(firstTask.Status);
				Sleep(TimeSpan.FromSeconds(0.5));
			}
			WriteLine(firstTask.Status);

			Sleep(TimeSpan.FromSeconds(10));
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
