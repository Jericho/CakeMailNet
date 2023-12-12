using Microsoft.Extensions.Logging;

namespace CakeMailNet
{
	/// <summary>
	/// Options for the CakeMailNet client.
	/// </summary>
	public class CakeMailNetClientOptions
	{
		/// <summary>
		/// Gets or sets the log levels for successful calls (HTTP status code in the 200-299 range).
		/// </summary>
		public LogLevel LogLevelSuccessfulCalls { get; set; }

		/// <summary>
		/// Gets or sets the log levels for failed calls (HTTP status code outside of the 200-299 range).
		/// </summary>
		public LogLevel LogLevelFailedCalls { get; set; }
	}
}
