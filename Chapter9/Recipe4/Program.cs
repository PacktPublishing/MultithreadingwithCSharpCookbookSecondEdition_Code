using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using static System.Console;

namespace Chapter9.Recipe4
{
	class Program
	{
		static void Main(string[] args)
		{
			ServiceHost host = null;

			try
			{
				host = new ServiceHost(typeof (HelloWorldService), new Uri(SERVICE_URL));
				var metadata = host.Description.Behaviors.Find<ServiceMetadataBehavior>() 
                    ?? new ServiceMetadataBehavior();

			    metadata.HttpGetEnabled = true;
				metadata.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
				host.Description.Behaviors.Add(metadata);

				host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, 
                    MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

				var endpoint = host.AddServiceEndpoint(typeof (IHelloWorldService),
                    new BasicHttpBinding(), SERVICE_URL);

				host.Faulted += (sender, e) => WriteLine("Error!");

				host.Open();

				WriteLine("Greeting service is running and listening on:");
				WriteLine($"{endpoint.Address} ({endpoint.Binding.Name})");

				var client = RunServiceClient();
				client.GetAwaiter().GetResult();

				WriteLine("Press Enter to exit");
				ReadLine();
			}
			catch (Exception ex)
			{
				WriteLine($"Error in catch block: {ex}");
			}
			finally
			{
				if (null != host)
				{
					if (host.State == CommunicationState.Faulted)
					{
						host.Abort();
					}
					else
					{
						host.Close();
					}
				}
			}
		}

		const string SERVICE_URL = "http://localhost:1234/HelloWorld";

		static async Task RunServiceClient()
		{
			var endpoint = new EndpointAddress(SERVICE_URL);
			var channel = ChannelFactory<IHelloWorldServiceClient>
                .CreateChannel(new BasicHttpBinding(), endpoint);

			var greeting = await channel.GreetAsync("Eugene");
			WriteLine(greeting);
		}

		[ServiceContract(Namespace = "Packt", Name = "HelloWorldServiceContract")]
		public interface IHelloWorldService
		{
			[OperationContract]
			string Greet(string name);
		}

		[ServiceContract(Namespace = "Packt", Name = "HelloWorldServiceContract")]
		public interface IHelloWorldServiceClient
		{
			[OperationContract]
			string Greet(string name);

			[OperationContract]
			Task<string> GreetAsync(string name);
		}

		public class HelloWorldService : IHelloWorldService
		{
			public string Greet(string name)
			{
				return $"Greetings, {name}!";
			}
		}
	}
}