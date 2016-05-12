using System;
using System.Threading;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter3.Recipe4
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var cts = new CancellationTokenSource())
			{
				CancellationToken token = cts.Token;
				ThreadPool.QueueUserWorkItem(_ => AsyncOperation1(token));
				Sleep(TimeSpan.FromSeconds(2));
				cts.Cancel();
			}

			using (var cts = new CancellationTokenSource())
			{
				CancellationToken token = cts.Token;
				ThreadPool.QueueUserWorkItem(_ => AsyncOperation2(token));
				Sleep(TimeSpan.FromSeconds(2));
				cts.Cancel();
			}

			using (var cts = new CancellationTokenSource())
			{
				CancellationToken token = cts.Token;
				ThreadPool.QueueUserWorkItem(_ => AsyncOperation3(token));
				Sleep(TimeSpan.FromSeconds(2));
				cts.Cancel();
			}

			Sleep(TimeSpan.FromSeconds(2));
		}

		static void AsyncOperation1(CancellationToken token)
		{
			WriteLine("Starting the first task");
			for (int i = 0; i < 5; i++)
			{
				if (token.IsCancellationRequested)
				{
					WriteLine("The first task has been canceled.");
					return;
				}
				Sleep(TimeSpan.FromSeconds(1));
			}
			WriteLine("The first task has completed succesfully");
		}

		static void AsyncOperation2(CancellationToken token)
		{
			try
			{
				WriteLine("Starting the second task");

				for (int i = 0; i < 5; i++)
				{
					token.ThrowIfCancellationRequested();
					Sleep(TimeSpan.FromSeconds(1));
				}
				WriteLine("The second task has completed succesfully");
			}
			catch (OperationCanceledException)
			{
				WriteLine("The second task has been canceled.");
			}
		}

		static void AsyncOperation3(CancellationToken token)
		{
			bool cancellationFlag = false;
			token.Register(() => cancellationFlag = true);
			WriteLine("Starting the third task");
			for (int i = 0; i < 5; i++)
			{
				if (cancellationFlag)
				{
					WriteLine("The third task has been canceled.");
					return;
				}
				Sleep(TimeSpan.FromSeconds(1));
			}
			WriteLine("The third task has completed succesfully");
		}
	}
}
