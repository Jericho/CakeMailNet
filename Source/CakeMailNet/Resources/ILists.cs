using CakeMailNet.Models;
using System.Threading;
using System.Threading.Tasks;

namespace CakeMailNet.Resources
{
	/// <summary>
	/// Allows you to manage lists.
	/// </summary>
	public interface ILists
	{
		/// <summary>
		/// Retrieve all lists.
		/// </summary>
		/// <param name="recordsPerPage">The number of records returned within a single API call.</param>
		/// <param name="page">The current page number of returned records.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		/// An array of <see cref="List">lists</see>.
		/// </returns>
		Task<PaginatedResponse<List>> GetAllAsync(int recordsPerPage = 30, int page = 1, CancellationToken cancellationToken = default);
	}
}
