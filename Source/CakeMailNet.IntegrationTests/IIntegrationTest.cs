using CakeMailNet;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CakeNet.IntegrationTests
{
	public interface IIntegrationTest
	{
		Task RunAsync(ICakeMailNetClient client, TextWriter log, CancellationToken cancellationToken);
	}
}
