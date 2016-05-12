using System;
using System.Threading.Tasks;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter4.Recipe4
{
	class Program
	{
		static void Main(string[] args)
		{
			int threadId;
			AsynchronousTask d = Test;
			IncompatibleAsynchronousTask e = Test;

			WriteLine("Option 1");
			Task<string> task = Task<string>.Factory.FromAsync(
				d.BeginInvoke("AsyncTaskThread", Callback, 
                "a delegate asynchronous call"), d.EndInvoke);

			task.ContinueWith(t => WriteLine(
                $"Callback is finished, now running a continuation! Result: {t.Result}"));

			while (!task.IsCompleted)
			{
				WriteLine(task.Status);
				Sleep(TimeSpan.FromSeconds(0.5));
			}
			WriteLine(task.Status);
			Sleep(TimeSpan.FromSeconds(1));

			WriteLine("----------------------------------------------");
			WriteLine();
			WriteLine("Option 2");

			task = Task<string>.Factory.FromAsync(
				d.BeginInvoke, d.EndInvoke, "AsyncTaskThread", "a delegate asynchronous call");

			task.ContinueWith(t => WriteLine(
                $"Task is completed, now running a continuation! Result: {t.Result}"));
			while (!task.IsCompleted)
			{
				WriteLine(task.Status);
				Sleep(TimeSpan.FromSeconds(0.5));
			}
			WriteLine(task.Status);
			Sleep(TimeSpan.FromSeconds(1));

			WriteLine("----------------------------------------------");
			WriteLine();
			WriteLine("Option 3");

			IAsyncResult ar = e.BeginInvoke(out threadId, Callback, "a delegate asynchronous call");
			task = Task<string>.Factory.FromAsync(ar, _ => e.EndInvoke(out threadId, ar));

			task.ContinueWith(t => 
				WriteLine(
                    $"Task is completed, now running a continuation! " +
                    $"Result: {t.Result}, ThreadId: {threadId}"));

			while (!task.IsCompleted)
			{
				WriteLine(task.Status);
				Sleep(TimeSpan.FromSeconds(0.5));
			}
			WriteLine(task.Status);

			Sleep(TimeSpan.FromSeconds(1));
		}

		delegate string AsynchronousTask(string threadName);
		delegate string IncompatibleAsynchronousTask(out int threadId);

		static void Callback(IAsyncResult ar)
		{
			WriteLine("Starting a callback...");
			WriteLine($"State passed to a callbak: {ar.AsyncState}");
			WriteLine($"Is thread pool thread: {CurrentThread.IsThreadPoolThread}");
			WriteLine($"Thread pool worker thread id: {CurrentThread.ManagedThreadId}");
		}

		static string Test(string threadName)
		{
			WriteLine("Starting...");
			WriteLine($"Is thread pool thread: {CurrentThread.IsThreadPoolThread}");
			Sleep(TimeSpan.FromSeconds(2));
			CurrentThread.Name = threadName;
			return $"Thread name: {CurrentThread.Name}";
		}

		static string Test(out int threadId)
		{
			WriteLine("Starting...");
			WriteLine($"Is thread pool thread: {CurrentThread.IsThreadPoolThread}");
			Sleep(TimeSpan.FromSeconds(2));
			threadId = CurrentThread.ManagedThreadId;
			return $"Thread pool worker thread id was: {threadId}";
		}
	}
}