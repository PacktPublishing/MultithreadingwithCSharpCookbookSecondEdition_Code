using System;
using System.Threading.Tasks;
using static System.Console;

namespace Chapter5.Recipe5
{
	class Program
	{
		static void Main(string[] args)
		{
			Task t = AsynchronousProcessing();
			t.Wait();
		}

		static async Task AsynchronousProcessing()
		{
			WriteLine("1. Single exception");

			try
			{
				string result = await GetInfoAsync("Task 1", 2);
				WriteLine(result);
			}
			catch (Exception ex)
			{
				WriteLine($"Exception details: {ex}");
			}

			WriteLine();
			WriteLine("2. Multiple exceptions");

			Task<string> t1 = GetInfoAsync("Task 1", 3);
			Task<string> t2 = GetInfoAsync("Task 2", 2);
			try
			{
				string[] results = await Task.WhenAll(t1, t2);
				WriteLine(results.Length);
			}
			catch (Exception ex)
			{
				WriteLine($"Exception details: {ex}");
			}

			WriteLine();
			WriteLine("3. Multiple exceptions with AggregateException");

			t1 = GetInfoAsync("Task 1", 3);
			t2 = GetInfoAsync("Task 2", 2);
			Task<string[]> t3 = Task.WhenAll(t1, t2);
			try
			{
				string[] results = await t3;
				WriteLine(results.Length);
			}
			catch
			{
				var ae = t3.Exception.Flatten();
				var exceptions = ae.InnerExceptions;
				WriteLine($"Exceptions caught: {exceptions.Count}");
				foreach (var e in exceptions)
				{
					WriteLine($"Exception details: {e}");
					WriteLine();
				}
			}

            WriteLine();
            WriteLine("4. await in catch and finally blocks");

		    try
		    {
		        string result = await GetInfoAsync("Task 1", 2);
		        WriteLine(result);
		    }
		    catch (Exception ex)
		    {
		        await Task.Delay(TimeSpan.FromSeconds(1));
		        WriteLine($"Catch block with await: Exception details: {ex}");
		    }
		    finally
		    {
                await Task.Delay(TimeSpan.FromSeconds(1));
                WriteLine("Finally block");
		    }
        }

		static async Task<string> GetInfoAsync(string name, int seconds)
		{
			await Task.Delay(TimeSpan.FromSeconds(seconds));
			throw new Exception($"Boom from {name}!");
		}
	}
}
