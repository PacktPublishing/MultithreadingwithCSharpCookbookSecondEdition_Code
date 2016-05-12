using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Console;

namespace Chapter6.Recipe1
{
	class Program
	{
        static void Main(string[] args)
		{
			var concurrentDictionary = new ConcurrentDictionary<int, string>();
			var dictionary = new Dictionary<int, string>();

			var sw = new Stopwatch();

			sw.Start();
			for (int i = 0; i < Iterations; i++)
			{
				lock (dictionary)
				{
					dictionary[i] = Item;
				}
			}
			sw.Stop();
			WriteLine($"Writing to dictionary with a lock: {sw.Elapsed}");

			sw.Restart();
			for (int i = 0; i < Iterations; i++)
			{
				concurrentDictionary[i] = Item;
			}
			sw.Stop();
			WriteLine($"Writing to a concurrent dictionary: {sw.Elapsed}");

			sw.Restart();
			for (int i = 0; i < Iterations; i++)
			{
				lock (dictionary)
				{
					CurrentItem = dictionary[i];
				}
			}
			sw.Stop();
			WriteLine($"Reading from dictionary with a lock: {sw.Elapsed}");

			sw.Restart();
			for (int i = 0; i < Iterations; i++)
			{
				CurrentItem = concurrentDictionary[i];
			}
			sw.Stop();
			WriteLine($"Reading from a concurrent dictionary: {sw.Elapsed}");
		}

		const string Item = "Dictionary item";
        const int Iterations = 1000000;
        public static string CurrentItem;
	}
}
