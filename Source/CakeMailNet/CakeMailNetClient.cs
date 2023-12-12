using CakeMailNet.Json;
using CakeMailNet.Resources;
using CakeMailNet.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace CakeMailNet
{
	/// <summary>
	/// REST client for interacting with CakeMail's Next-Gen API.
	/// </summary>
	public class CakeMailNetClient : ICakeMailNetClient, IDisposable
	{
		#region FIELDS

		private const string CAKEMAIL_NEXT_GEN_API_BASE_URI = "https://api.cakemail.dev";

		private readonly bool _mustDisposeHttpClient;
		private readonly CakeMailNetClientOptions _options;
		private readonly ILogger _logger;

		private HttpClient _httpClient;
		private Pathoschild.Http.Client.IClient _fluentClient;

		#endregion

		#region PROPERTIES

		/// <summary>
		/// Gets the Version.
		/// </summary>
		/// <value>
		/// The version.
		/// </value>
		public static string Version { get; private set; }

		/// <inheritdoc/>
		public ILists Lists { get; private set; }

		/*
		/// <summary>
		/// Gets the resource which allows you to manage chat channels, messages, etc.
		/// </summary>
		/// <value>
		/// The chat resource.
		/// </value>
		public IChat Chat { get; private set; }

		/// <summary>
		/// Gets the resource which allows you to manage cloud recordings.
		/// </summary>
		/// <value>
		/// The recordings resource.
		/// </value>
		public ICloudRecordings CloudRecordings { get; private set; }

		/// <summary>
		/// Gets the resource which allows you to manage contacts.
		/// </summary>
		/// <value>
		/// The contacts resource.
		/// </value>
		public IContacts Contacts { get; private set; }

		/// <summary>
		/// Gets the resource which allows you to notify Zoom that you comply with the policy which requires
		/// you to handle user's data in accordance to the user's preference after the user uninstalls your app.
		/// </summary>
		/// <value>
		/// The data compliance resource.
		/// </value>
		[Obsolete("The Data Compliance API is deprecated")]
		public IDataCompliance DataCompliance { get; private set; }

		/// <summary>
		/// Gets the resource which allows you to manage meetings.
		/// </summary>
		/// <value>
		/// The meetings resource.
		/// </value>
		public IMeetings Meetings { get; private set; }

		/// <summary>
		/// Gets the resource which allows you to manage meetings that occured in the past.
		/// </summary>
		/// <value>
		/// The past meetings resource.
		/// </value>
		public IPastMeetings PastMeetings { get; private set; }

		/// <summary>
		/// Gets the resource which allows you to manage webinars that occured in the past.
		/// </summary>
		/// <value>
		/// The past webinars resource.
		/// </value>
		public IPastWebinars PastWebinars { get; private set; }

		/// <summary>
		/// Gets the resource which allows you to manage roles.
		/// </summary>
		/// <value>
		/// The roles resource.
		/// </value>
		public IRoles Roles { get; private set; }

		/// <summary>
		/// Gets the resource which allows you to manage users.
		/// </summary>
		/// <value>
		/// The users resource.
		/// </value>
		public IUsers Users { get; private set; }

		/// <summary>
		/// Gets the resource which allows you to manage webinars.
		/// </summary>
		/// <value>
		/// The webinars resource.
		/// </value>
		public IWebinars Webinars { get; private set; }

		/// <summary>
		/// Gets the resource which allows you to view metrics.
		/// </summary>
		public IDashboards Dashboards { get; private set; }

		/// <summary>
		/// Gets the resource which allows you to view reports.
		/// </summary>
		public IReports Reports { get; private set; }

		/// <summary>
		/// Gets the resource which allows you to manage call logs.
		/// </summary>
		public ICallLogs CallLogs { get; private set; }

		/// <summary>
		/// Gets the resource which allows you to manage chatbot messages.
		/// </summary>
		public IChatbot Chatbot { get; private set; }

		/// <inheritdoc/>
		public IPhone Phone { get; private set; }
		*/

		#endregion

		#region CTOR

		static CakeMailNetClient()
		{
			Version = typeof(CakeMailNetClient).GetTypeInfo().Assembly.GetName().Version.ToString(3);
#if DEBUG
			Version = "DEBUG";
#endif
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CakeMailNetClient"/> class.
		/// </summary>
		/// <param name="connectionInfo">Connection information.</param>
		/// <param name="options">Options for the CakeMailNet client.</param>
		/// <param name="logger">Logger.</param>
		public CakeMailNetClient(IConnectionInfo connectionInfo, CakeMailNetClientOptions options = null, ILogger logger = null)
			: this(connectionInfo, new HttpClient(), true, options, logger)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CakeMailNetClient"/> class with a specific proxy.
		/// </summary>
		/// <param name="connectionInfo">Connection information.</param>
		/// <param name="proxy">Allows you to specify a proxy.</param>
		/// <param name="options">Options for the CakeMailNet client.</param>
		/// <param name="logger">Logger.</param>
		public CakeMailNetClient(IConnectionInfo connectionInfo, IWebProxy proxy, CakeMailNetClientOptions options = null, ILogger logger = null)
			: this(connectionInfo, new HttpClient(new HttpClientHandler { Proxy = proxy, UseProxy = proxy != null }), true, options, logger)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CakeMailNetClient"/> class with a specific handler.
		/// </summary>
		/// <param name="connectionInfo">Connection information.</param>
		/// <param name="handler">TThe HTTP handler stack to use for sending requests.</param>
		/// <param name="options">Options for the CakeMailNet client.</param>
		/// <param name="logger">Logger.</param>
		public CakeMailNetClient(IConnectionInfo connectionInfo, HttpMessageHandler handler, CakeMailNetClientOptions options = null, ILogger logger = null)
			: this(connectionInfo, new HttpClient(handler), true, options, logger)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CakeMailNetClient"/> class with a specific http client.
		/// </summary>
		/// <param name="connectionInfo">Connection information.</param>
		/// <param name="httpClient">Allows you to inject your own HttpClient. This is useful, for example, to setup the HtppClient with a proxy.</param>
		/// <param name="options">Options for the CakeMailNet client.</param>
		/// <param name="logger">Logger.</param>
		public CakeMailNetClient(IConnectionInfo connectionInfo, HttpClient httpClient, CakeMailNetClientOptions options = null, ILogger logger = null)
			: this(connectionInfo, httpClient, false, options, logger)
		{
		}

		private CakeMailNetClient(IConnectionInfo connectionInfo, HttpClient httpClient, bool disposeClient, CakeMailNetClientOptions options, ILogger logger = null)
		{
			_mustDisposeHttpClient = disposeClient;
			_httpClient = httpClient;
			_options = options ?? GetDefaultOptions();
			_logger = logger ?? NullLogger.Instance;
			_fluentClient = new FluentClient(new Uri(CAKEMAIL_NEXT_GEN_API_BASE_URI), httpClient)
				.SetUserAgent($"CakeMailNet/{Version} (+https://github.com/Jericho/CakeMailNet)");

			_fluentClient.Filters.Remove<DefaultErrorFilter>();

			// Remove all the built-in formatters and replace them with our custom JSON formatter
			_fluentClient.Formatters.Clear();
			_fluentClient.Formatters.Add(new JsonFormatter());

			// Order is important: the token handler must be first, followed by DiagnosticHandler and then by ErrorHandler.
			if (connectionInfo is OAuthConnectionInfo oAuthConnectionInfo)
			{
				var tokenHandler = new OAuthTokenHandler(oAuthConnectionInfo, httpClient);
				_fluentClient.Filters.Add(tokenHandler);
				_fluentClient.SetRequestCoordinator(new CakeMailRetryCoordinator(new Http429RetryStrategy(), tokenHandler));
			}
			else
			{
				throw new CakeMailException($"{connectionInfo.GetType()} is an unknown connection type", null, null, null);
			}

			// The list of filters must be kept in sync with the filters in Utils.GetFluentClient in the unit testing project.
			_fluentClient.Filters.Add(new DiagnosticHandler(_options.LogLevelSuccessfulCalls, _options.LogLevelFailedCalls, _logger));
			_fluentClient.Filters.Add(new CakeMailErrorHandler());

			Lists = new Lists(_fluentClient);
			/*
			Accounts = new Accounts(_fluentClient);
			Chat = new Chat(_fluentClient);
			CloudRecordings = new CloudRecordings(_fluentClient);
			Contacts = new Contacts(_fluentClient);
			DataCompliance = new DataCompliance(_fluentClient);
			Meetings = new Meetings(_fluentClient);
			PastMeetings = new PastMeetings(_fluentClient);
			PastWebinars = new PastWebinars(_fluentClient);
			Roles = new Roles(_fluentClient);
			Users = new Users(_fluentClient);
			Webinars = new Webinars(_fluentClient);
			Dashboards = new Dashboards(_fluentClient);
			Reports = new Reports(_fluentClient);
			CallLogs = new CallLogs(_fluentClient);
			Chatbot = new Chatbot(_fluentClient);
			Phone = new Phone(_fluentClient);
			*/
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="CakeMailNetClient"/> class.
		/// </summary>
		~CakeMailNetClient()
		{
			// The object went out of scope and finalized is called.
			// Call 'Dispose' to release unmanaged resources
			// Managed resources will be released when GC runs the next time.
			Dispose(false);
		}

		#endregion

		#region PUBLIC METHODS

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			// Call 'Dispose' to release resources
			Dispose(true);

			// Tell the GC that we have done the cleanup and there is nothing left for the Finalizer to do
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				ReleaseManagedResources();
			}
			else
			{
				// The object went out of scope and the Finalizer has been called.
				// The GC will take care of releasing managed resources, therefore there is nothing to do here.
			}

			ReleaseUnmanagedResources();
		}

		#endregion

		#region PRIVATE METHODS

		private static CakeMailNetClientOptions GetDefaultOptions()
		{
			return new CakeMailNetClientOptions()
			{
				LogLevelSuccessfulCalls = LogLevel.Debug,
				LogLevelFailedCalls = LogLevel.Error
			};
		}

		private void ReleaseManagedResources()
		{
			if (_fluentClient != null)
			{
				_fluentClient.Dispose();
				_fluentClient = null;
			}

			if (_httpClient != null && _mustDisposeHttpClient)
			{
				_httpClient.Dispose();
				_httpClient = null;
			}
		}

		private void ReleaseUnmanagedResources()
		{
			// We do not hold references to unmanaged resources
		}

		#endregion
	}
}
