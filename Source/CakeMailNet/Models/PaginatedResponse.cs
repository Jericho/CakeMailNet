using System.Text.Json.Serialization;

namespace CakeMailNet.Models
{
	/// <summary>
	/// Pagination Object.
	/// </summary>
	/// <typeparam name="T">The type of records.</typeparam>
	public class PaginatedResponse<T>
	{
		/// <summary>
		/// Gets or sets the page number of current results.
		/// </summary>
		/// <value>The page number of current results.</value>
		[JsonPropertyName("page")]
		public int PageNumber { get; set; }

		/// <summary>
		/// Gets or sets the number of records returned within a single API call.
		/// </summary>
		/// <value>The number of records returned within a single API call.</value>
		[JsonPropertyName("per_page")]
		public int PageSize { get; set; }

		/// <summary>
		/// Gets or sets the number of all records available across pages.
		/// </summary>
		/// <value>The number of all records available across pages.</value>
		[JsonPropertyName("count")]
		public int TotalRecords { get; set; }

		/// <summary>
		/// Gets or sets the records.
		/// </summary>
		/// <value>The records.</value>
		public T[] Records { get; set; }
	}
}
