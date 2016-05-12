using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter10.Recipe2
{
	class Program
	{
		static void Main(string[] args)
		{
			var cts = new CancellationTokenSource();

            Task.Run(() =>
			{
				if (ReadKey().KeyChar == 'c') cts.Cancel();
			}, 
            cts.Token);

			var sourceArrays = new BlockingCollection<int>[CollectionsNumber];

			for (int i = 0; i < sourceArrays.Length; i++)
			{
				sourceArrays[i] = new BlockingCollection<int>(Count);
			}

			var convertToDecimal = new PipelineWorker<int, decimal>
			(
				sourceArrays,
				n => Convert.ToDecimal(n*100),
				cts.Token,
				"Decimal Converter"
			);

			var stringifyNumber = new PipelineWorker<decimal, string>
			(
				convertToDecimal.Output,
				s => $"--{s.ToString("C", CultureInfo.GetCultureInfo("en-us"))}--",
				cts.Token,
				"String Formatter"
			 );

			var outputResultToConsole = new PipelineWorker<string, string>
			(
				stringifyNumber.Output,
				s => WriteLine($"The final result is {s} on thread " +
				               $"id {CurrentThread.ManagedThreadId}"),
				cts.Token,
				"Console Output"
			 );

			try
			{
				Parallel.Invoke(
					() =>
					CreateInitialValues(sourceArrays, cts),
					() => convertToDecimal.Run(),
					() => stringifyNumber.Run(),
					() => outputResultToConsole.Run()
				);
			}
			catch (AggregateException ae)
			{
				foreach (var ex in ae.InnerExceptions)
					WriteLine(ex.Message + ex.StackTrace);
			}

			if (cts.Token.IsCancellationRequested)
			{
				WriteLine("Operation has been canceled! Press ENTER to exit.");
			}
			else
			{
				WriteLine("Press ENTER to exit.");
			}
			ReadLine();
		}

        const int CollectionsNumber = 4;
        const int Count = 5;

        static void CreateInitialValues(BlockingCollection<int>[] sourceArrays, 
            CancellationTokenSource cts)
	    {
	        Parallel.For(0, sourceArrays.Length*Count, (j, state) =>
	        {
	            if (cts.Token.IsCancellationRequested)
	            {
	                state.Stop();
	            }

	            int number = GetRandomNumber(j);
	            int k = BlockingCollection<int>.TryAddToAny(sourceArrays, j);
	            if (k >= 0)
	            {
	                WriteLine(
	                    $"added {j} to source data on thread " +
	                    $"id {CurrentThread.ManagedThreadId}");
	                Sleep(TimeSpan.FromMilliseconds(number));
	            }
	        });
	        foreach (var arr in sourceArrays)
	        {
	            arr.CompleteAdding();
	        }
	    }

	    static int GetRandomNumber(int seed)
	    {
	        return new Random(seed).Next(500);
	    }

		class PipelineWorker<TInput, TOutput>
		{
			Func<TInput, TOutput> _processor;
			Action<TInput> _outputProcessor;
			BlockingCollection<TInput>[] _input;
			CancellationToken _token;
		    Random _rnd;

			public PipelineWorker(
					BlockingCollection<TInput>[] input,
					Func<TInput, TOutput> processor,
					CancellationToken token,
					string name)
			{
				_input = input;
				Output = new BlockingCollection<TOutput>[_input.Length];
				for (int i = 0; i < Output.Length; i++)
					Output[i] = null == input[i] ? null : new BlockingCollection<TOutput>(Count);

				_processor = processor;
				_token = token;
				Name = name;
                _rnd = new Random(DateTime.Now.Millisecond);
			}

			public PipelineWorker(
					BlockingCollection<TInput>[] input,
					Action<TInput> renderer,
					CancellationToken token,
					string name)
			{
				_input = input;
				_outputProcessor = renderer;
				_token = token;
				Name = name;
				Output = null;
                _rnd = new Random(DateTime.Now.Millisecond);
            }

			public BlockingCollection<TOutput>[] Output { get; private set; }

			public string Name { get; private set; }

			public void Run()
			{
				WriteLine($"{Name} is running");
				while (!_input.All(bc => bc.IsCompleted) && !_token.IsCancellationRequested)
				{
					TInput receivedItem;
					int i = BlockingCollection<TInput>.TryTakeFromAny(
							_input, out receivedItem, 50, _token);
					if (i >= 0)
					{
						if (Output != null)
						{
							TOutput outputItem = _processor(receivedItem);
							BlockingCollection<TOutput>.AddToAny(Output, outputItem);
							WriteLine($"{Name} sent {outputItem} to next, on " +
							          $"thread id {CurrentThread.ManagedThreadId}");
							Sleep(TimeSpan.FromMilliseconds(_rnd.Next(200)));
						}
						else
						{
							_outputProcessor(receivedItem);
						}
					}
					else
					{
						Sleep(TimeSpan.FromMilliseconds(50));
					}
				}
				if (Output != null)
				{
					foreach (var bc in Output) bc.CompleteAdding();
				}
			}
		}
	}
}
