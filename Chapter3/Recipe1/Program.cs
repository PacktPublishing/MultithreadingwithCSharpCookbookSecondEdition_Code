using System;
using System.Threading;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter3.Recipe1
{
	class Program
	{
		static void Main(string[] args)
		{
			int threadId = 0;

			RunOnThreadPool poolDelegate = Test;

			var t = new Thread(() => Test(out threadId));
			t.Start();
			t.Join();

			WriteLine($"Thread id: {threadId}");

			IAsyncResult r = poolDelegate.BeginInvoke(out threadId, Callback, "a delegate asynchronous call");
			r.AsyncWaitHandle.WaitOne();

			string result = poolDelegate.EndInvoke(out threadId, r);
			
			WriteLine($"Thread pool worker thread id: {threadId}");
			WriteLine(result);

			Sleep(TimeSpan.FromSeconds(2));
		}

		private delegate string RunOnThreadPool(out int threadId);

		private static void Callback(IAsyncResult ar)
		{
			WriteLine("Starting a callback...");
			WriteLine($"State passed to a callbak: {ar.AsyncState}");
			WriteLine($"Is thread pool thread: {CurrentThread.IsThreadPoolThread}");
			WriteLine($"Thread pool worker thread id: {CurrentThread.ManagedThreadId}");
		}


		private static string Test(out int threadId)
		{
			WriteLine("Starting...");
			WriteLine($"Is thread pool thread: {CurrentThread.IsThreadPoolThread}");
			Sleep(TimeSpan.FromSeconds(2));
			threadId = CurrentThread.ManagedThreadId;
			return $"Thread pool worker thread id was: {threadId}";
		}
	}
}
