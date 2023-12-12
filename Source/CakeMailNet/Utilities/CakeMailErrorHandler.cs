using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;

namespace CakeMailNet.Utilities
{
	/// <summary>
	/// Error handler for requests dispatched to the CakeMail next-gen API.
	/// </summary>
	/// <seealso cref="Pathoschild.Http.Client.Extensibility.IHttpFilter" />
	internal class CakeMailErrorHandler : IHttpFilter
	{
		public CakeMailErrorHandler()
		{ }

		#region PUBLIC METHODS

		/// <summary>Method invoked just before the HTTP request is submitted. This method can modify the outgoing HTTP request.</summary>
		/// <param name="request">The HTTP request.</param>
		public void OnRequest(IRequest request) { }

		/// <summary>Method invoked just after the HTTP response is received. This method can modify the incoming HTTP response.</summary>
		/// <param name="response">The HTTP response.</param>
		/// <param name="httpErrorAsException">Whether HTTP error responses should be raised as exceptions.</param>
		public void OnResponse(IResponse response, bool httpErrorAsException)
		{
			var (isError, errorMessage) = response.Message.GetErrorMessageAsync().ConfigureAwait(false).GetAwaiter().GetResult();

			if (isError)
			{
				var diagnosticInfo = response.GetDiagnosticInfo();
				var diagnosticLog = diagnosticInfo.Diagnostic ?? "Diagnostic log unavailable";

				throw new CakeMailException(errorMessage, response.Message, diagnosticLog);
			}
		}

		#endregion
	}
}
