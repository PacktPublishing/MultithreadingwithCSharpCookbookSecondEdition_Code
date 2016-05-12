using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter7.Recipe6
{
	class Program
	{
		static void Main(string[] args)
		{
			var parallelQuery = from t in GetTypes().AsParallel()
                                select t;

			var parallelAggregator = parallelQuery.Aggregate(
				() => new ConcurrentDictionary<char, int>(),
				(taskTotal, item) => AccumulateLettersInformation(taskTotal, item), 
				(total, taskTotal) => MergeAccumulators(total, taskTotal),
				total => total);

			WriteLine();
			WriteLine("There were the following letters in type names:");
			var orderedKeys = from k in parallelAggregator.Keys
							  orderby parallelAggregator[k] descending
							  select k;

			foreach (var c in orderedKeys)
			{
				WriteLine($"Letter '{c}' ---- {parallelAggregator[c]} times");
			}
		}

		static ConcurrentDictionary<char, int> AccumulateLettersInformation(
            ConcurrentDictionary<char, int> taskTotal , string item)
		{
			foreach (var c in item)
			{
				if (taskTotal.ContainsKey(c))
				{
					taskTotal[c] = taskTotal[c] + 1;
				}
				else
				{
					taskTotal[c] = 1;
				}
			}
			WriteLine($"{item} type was aggregated on a thread " +
			          $"id {CurrentThread.ManagedThreadId}");
			return taskTotal;
		}

		static ConcurrentDictionary<char, int> MergeAccumulators(
            ConcurrentDictionary<char, int> total, ConcurrentDictionary<char, int> taskTotal)
		{
			foreach (var key in taskTotal.Keys)
			{
				if (total.ContainsKey(key))
				{
					total[key] = total[key] + taskTotal[key];
				}
				else
				{
					total[key] = taskTotal[key];
				}
			}
			WriteLine("---");
			WriteLine($"Total aggregate value was calculated on a thread " +
			          $"id {CurrentThread.ManagedThreadId}");
			return total;
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
	}
}
