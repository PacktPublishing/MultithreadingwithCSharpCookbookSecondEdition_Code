using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace Chapter7.Recipe4
{
	class Program
	{
		static void Main(string[] args)
		{
			IEnumerable<int> numbers = Enumerable.Range(-5, 10);

			var query = from number in numbers
						select 100 / number;

			try
			{
				foreach(var n in query)
					WriteLine(n);
			}
			catch (DivideByZeroException)
			{
				WriteLine("Divided by zero!");
			}

			WriteLine("---");
			WriteLine("Sequential LINQ query processing");
			WriteLine();

			var parallelQuery = from number in numbers.AsParallel()
								select 100 / number; ;

			try
			{
				parallelQuery.ForAll(WriteLine);
			}
			catch (DivideByZeroException)
			{
				WriteLine("Divided by zero - usual exception handler!");
			}
			catch (AggregateException e)
			{
				e.Flatten().Handle(ex =>
				{
					if (ex is DivideByZeroException)
					{
						WriteLine("Divided by zero - aggregate exception handler!");
						return true;
					}
					
					return false;
				});
			}

			WriteLine("---");
			WriteLine("Parallel LINQ query processing and results merging");
		}
	}
}
