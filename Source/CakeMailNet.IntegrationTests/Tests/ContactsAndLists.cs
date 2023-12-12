using CakeMailNet;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CakeNet.IntegrationTests.Tests
{
	public class ContactsAndLists : IIntegrationTest
	{
		public async Task RunAsync(ICakeMailNetClient client, TextWriter log, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			await log.WriteLineAsync("\n***** CONTACTS AND LISTS*****\n").ConfigureAwait(false);

			// GET ALL LISTS
			var lists = await client.Lists.GetAllAsync(50, 1, cancellationToken).ConfigureAwait(false);
			await log.WriteLineAsync($"All lists retrieved. There are {lists.TotalRecords} lists").ConfigureAwait(false);
		}
	}
}
