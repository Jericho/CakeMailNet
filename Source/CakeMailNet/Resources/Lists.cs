using CakeMailNet.Models;
using Pathoschild.Http.Client;
using System.Threading;
using System.Threading.Tasks;

namespace CakeMailNet.Resources
{
	/// <summary>
	/// Allows you to manage lists.
	/// </summary>
	/// <seealso cref="CakeMailNet.Resources.ILists" />
	public class Lists : ILists
	{
		private readonly Pathoschild.Http.Client.IClient _client;

		/// <summary>
		/// Initializes a new instance of the <see cref="Lists" /> class.
		/// </summary>
		/// <param name="client">The HTTP client.</param>
		internal Lists(Pathoschild.Http.Client.IClient client)
		{
			_client = client;
		}

		/// <inheritdoc/>
		public Task<PaginatedResponse<List>> GetAllAsync(int recordsPerPage = 30, int page = 1, CancellationToken cancellationToken = default)
		{
			return _client
				.GetAsync($"lists")
				.WithArgument("page", page)
				.WithArgument("per_page", recordsPerPage)
				.WithCancellationToken(cancellationToken)
				.AsPaginatedResponse<List>("data");
		}
	}
}
