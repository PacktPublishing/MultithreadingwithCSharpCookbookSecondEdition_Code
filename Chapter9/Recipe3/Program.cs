using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using static System.Console;

namespace Chapter9.Recipe3
{
	class Program
	{
		static void Main(string[] args)
		{
			const string dataBaseName = "CustomDatabase";
			var t = ProcessAsynchronousIO(dataBaseName);
			t.GetAwaiter().GetResult();
			WriteLine("Press Enter to exit");
			ReadLine();
		}

		static async Task ProcessAsynchronousIO(string dbName)
		{
			try
			{
				const string connectionString =
                    @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;" +
                    "Integrated Security=True";

				string outputFolder = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location);

				string dbFileName = Path.Combine(outputFolder, $"{dbName}.mdf");
				string dbLogFileName = Path.Combine(outputFolder, $"{dbName}_log.ldf");

				string dbConnectionString = 
					@"Data Source=(LocalDB)\MSSQLLocalDB;" +
                    $"AttachDBFileName={dbFileName};Integrated Security=True;";

				using (var connection = new SqlConnection(connectionString))
				{
					await connection.OpenAsync();

					if (File.Exists(dbFileName))
					{
						WriteLine("Detaching the database...");

						var detachCommand = new SqlCommand("sp_detach_db", connection);
						detachCommand.CommandType = CommandType.StoredProcedure;
						detachCommand.Parameters.AddWithValue("@dbname", dbName);

						await detachCommand.ExecuteNonQueryAsync();

						WriteLine("The database was detached succesfully.");
						WriteLine("Deleting the database...");

						if(File.Exists(dbLogFileName)) File.Delete(dbLogFileName);
						File.Delete(dbFileName);

						WriteLine("The database was deleted succesfully.");
					}

					WriteLine("Creating the database...");
					string createCommand = 
                        $"CREATE DATABASE {dbName} ON (NAME = N'{dbName}', FILENAME = " +
                        $"'{dbFileName}')";
					var cmd = new SqlCommand(createCommand, connection);

					await cmd.ExecuteNonQueryAsync();
					WriteLine("The database was created succesfully");
				}

				using (var connection = new SqlConnection(dbConnectionString))
				{
					await connection.OpenAsync();

					var cmd = new SqlCommand("SELECT newid()", connection);
					var result = await cmd.ExecuteScalarAsync();

					WriteLine($"New GUID from DataBase: {result}");

					cmd = new SqlCommand(
 @"CREATE TABLE [dbo].[CustomTable]( [ID] [int] IDENTITY(1,1) NOT NULL, " + 
 "[Name] [nvarchar](50) NOT NULL, CONSTRAINT [PK_ID] PRIMARY KEY CLUSTERED " + 
 " ([ID] ASC) ON [PRIMARY]) ON [PRIMARY]", connection);

                    await cmd.ExecuteNonQueryAsync();

					WriteLine("Table was created succesfully.");

					cmd = new SqlCommand(
@"INSERT INTO [dbo].[CustomTable] (Name) VALUES ('John');
INSERT INTO [dbo].[CustomTable] (Name) VALUES ('Peter');
INSERT INTO [dbo].[CustomTable] (Name) VALUES ('James');
INSERT INTO [dbo].[CustomTable] (Name) VALUES ('Eugene');", connection);
					await cmd.ExecuteNonQueryAsync();

					WriteLine("Inserted data succesfully");
					WriteLine("Reading data from table...");

					cmd = new SqlCommand(@"SELECT * FROM [dbo].[CustomTable]", connection);
					using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							var id = reader.GetFieldValue<int>(0);
							var name = reader.GetFieldValue<string>(1);

							WriteLine("Table row: Id {0}, Name {1}", id, name);
						}
					}
				}
			}
			catch(Exception ex)
			{
				WriteLine("Error: {0}", ex.Message);
			}
		}
	}
}
