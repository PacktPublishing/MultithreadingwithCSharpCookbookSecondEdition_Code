using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter10.Recipe3
{
	class Program
	{
		static void Main(string[] args)
		{
			var t = ProcessAsynchronously();
			t.GetAwaiter().GetResult();
		}

        async static Task ProcessAsynchronously()
		{
			var cts = new CancellationTokenSource();
            Random _rnd = new Random(DateTime.Now.Millisecond);

            Task.Run(() =>
			{
				if (ReadKey().KeyChar == 'c')
					cts.Cancel();
			}, cts.Token);

			var inputBlock = new BufferBlock<int>(
				new DataflowBlockOptions { BoundedCapacity = 5, CancellationToken = cts.Token });

			var convertToDecimalBlock = new TransformBlock<int, decimal>(
				n =>
				{
					decimal result = Convert.ToDecimal(n * 100);
					WriteLine($"Decimal Converter sent {result} to the next stage on " +
					          $"thread id {CurrentThread.ManagedThreadId}");
					Sleep(TimeSpan.FromMilliseconds(_rnd.Next(200)));
					return result;
				}
				, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4, CancellationToken = cts.Token });

			var stringifyBlock = new TransformBlock<decimal, string>(
				n =>
				{
					string result = $"--{n.ToString("C", CultureInfo.GetCultureInfo("en-us"))}--";
					WriteLine($"String Formatter sent {result} to the next stage on thread id {CurrentThread.ManagedThreadId}");
					Sleep(TimeSpan.FromMilliseconds(_rnd.Next(200)));
					return result;
				}
				, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4, CancellationToken = cts.Token });

			var outputBlock = new ActionBlock<string>(
				s =>
				{
					WriteLine($"The final result is {s} on thread id {CurrentThread.ManagedThreadId}");
				}
				, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 4, CancellationToken = cts.Token });

			inputBlock.LinkTo(convertToDecimalBlock, new DataflowLinkOptions { PropagateCompletion = true });
			convertToDecimalBlock.LinkTo(stringifyBlock, new DataflowLinkOptions { PropagateCompletion = true });
			stringifyBlock.LinkTo(outputBlock, new DataflowLinkOptions { PropagateCompletion = true });

			try
			{
				Parallel.For(0, 20, new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = cts.Token }
				, i =>
				{
					WriteLine($"added {i} to source data on thread id {CurrentThread.ManagedThreadId}");
					inputBlock.SendAsync(i).GetAwaiter().GetResult();
				});
				inputBlock.Complete();
				await outputBlock.Completion;
				WriteLine("Press ENTER to exit.");
			}
			catch (OperationCanceledException)
			{
				WriteLine("Operation has been canceled! Press ENTER to exit.");
			}

			ReadLine();
		}
	}
}
