using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.Console;
using static System.Threading.Thread;

namespace Chapter7.Recipe2
{
	class Program
	{
		static void Main(string[] args)
		{
			var sw = new Stopwatch();
			sw.Start();
			var query = from t in GetTypes()
									select EmulateProcessing(t);

			foreach (string typeName in query)
			{
				PrintInfo(typeName);
			}
			sw.Stop();
			WriteLine("---");
			WriteLine("Sequential LINQ query.");
			WriteLine($"Time elapsed: {sw.Elapsed}");
			WriteLine("Press ENTER to continue....");
			ReadLine();
			Clear();
			sw.Reset();

			sw.Start();
			var parallelQuery = from t in GetTypes().AsParallel()
								select EmulateProcessing(t);

			foreach (var typeName in parallelQuery)
			{
				PrintInfo(typeName);
			}
			sw.Stop();
			WriteLine("---");
			WriteLine("Parallel LINQ query. The results are being merged on a single thread");
			WriteLine($"Time elapsed: {sw.Elapsed}");
			WriteLine("Press ENTER to continue....");
			ReadLine();
			Clear();
			sw.Reset();

			sw.Start();
			parallelQuery = from t in GetTypes().AsParallel()
							select EmulateProcessing(t);

			parallelQuery.ForAll(PrintInfo);

			sw.Stop();
			WriteLine("---");
			WriteLine("Parallel LINQ query. The results are being processed in parallel");
			WriteLine($"Time elapsed: {sw.Elapsed}");
			WriteLine("Press ENTER to continue....");
			ReadLine();
			Clear();
			sw.Reset();

			sw.Start();
			query = from t in GetTypes().AsParallel().AsSequential()
					select EmulateProcessing(t);

			foreach (string typeName in query)
			{
				PrintInfo(typeName);
			}

			sw.Stop();
			WriteLine("---");
			WriteLine("Parallel LINQ query, transformed into sequential.");
			WriteLine($"Time elapsed: {sw.Elapsed}");
			WriteLine("Press ENTER to continue....");
			ReadLine();
			Clear();
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
			          $"id {CurrentThread.ManagedThreadId}");
			return typeName;
		}

		static IEnumerable<string> GetTypes()
		{
			return from assembly in AppDomain.CurrentDomain.GetAssemblies()
						 from type in assembly.GetExportedTypes()
						 where type.Name.StartsWith("Web")
						 select type.Name;

		}
	}
}
