using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter7.Recipe5
{
	class Program
	{
		static void Main(string[] args)
		{
			var timer = Stopwatch.StartNew();
			var partitioner = new StringPartitioner(GetTypes());
			var parallelQuery = from t in partitioner.AsParallel()
			//									.WithDegreeOfParallelism(1)
													select EmulateProcessing(t);

			parallelQuery.ForAll(PrintInfo);
			int count = parallelQuery.Count();
			timer.Stop();
			WriteLine(" ----------------------- ");
			WriteLine($"Total items processed: {count}");
			WriteLine($"Time elapsesd: {timer.Elapsed}");
		}

		static void PrintInfo(string typeName)
		{
			Sleep(TimeSpan.FromMilliseconds(150));
			WriteLine($"{typeName} type was printed on a thread " +
								$"id {CurrentThread.ManagedThreadId}");
		}

		static string EmulateProcessing(string typeName)
		{
			Sleep(TimeSpan.FromMilliseconds(150));
			WriteLine($"{typeName} type was processed on a thread " +
								$"id { CurrentThread.ManagedThreadId}. Has " +
								$"{(typeName.Length % 2 == 0 ? "even" : "odd")} length.");
			return typeName;
		}

		static IEnumerable<string> GetTypes()
		{
			var types = AppDomain.CurrentDomain
				.GetAssemblies()
				.SelectMany(a => a.GetExportedTypes());

			return from type in types
						 where type.Name.StartsWith("Web")
						 select type.Name;
		}

		public class StringPartitioner : Partitioner<string>
		{
			private readonly IEnumerable<string> _data;

			public StringPartitioner(IEnumerable<string> data)
			{
				_data = data;
			}

			public override bool SupportsDynamicPartitions => false;

			public override IList<IEnumerator<string>> GetPartitions(int partitionCount)
			{
				var result = new List<IEnumerator<string>>(partitionCount);

				for (int i = 1; i <= partitionCount; i++)
				{
					result.Add(CreateEnumerator(i, partitionCount));
				}

				return result;
			}

			IEnumerator<string> CreateEnumerator(int partitionNumber, int partitionCount)
			{
				int evenPartitions = partitionCount / 2;
				bool isEven = partitionNumber % 2 == 0;
				int step = isEven ? evenPartitions : partitionCount - evenPartitions;
				int startIndex = partitionNumber / 2 + partitionNumber % 2;

				var q = _data
						.Where(v => !(v.Length % 2 == 0 ^ isEven) || partitionCount == 1)
						.Skip(startIndex - 1);

				return q
						.Where((x, i) => i % step == 0)
						.GetEnumerator();

			}
		}
	}
}
