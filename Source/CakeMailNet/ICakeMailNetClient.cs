using CakeMailNet.Resources;

namespace CakeMailNet
{
	/// <summary>
	/// Interface for the CakeMailNet client.
	/// </summary>
	public interface ICakeMailNetClient
	{
		/// <summary>
		/// Gets the resource which allows you to manage lists.
		/// </summary>
		/// <value>
		/// The lists resource.
		/// </value>
		ILists Lists { get; }

		/*
		/// <summary>
		/// Gets the resource which allows you to manage chat channels, messages, etc.
		/// </summary>
		/// <value>
		/// The chat resource.
		/// </value>
		IChat Chat { get; }

		/// <summary>
		/// Gets the resource which allows you to manage cloud recordings.
		/// </summary>
		/// <value>
		/// The cloud recordings resource.
		/// </value>
		ICloudRecordings CloudRecordings { get; }

		/// <summary>
		/// Gets the resource which allows you to manage contacts.
		/// </summary>
		/// <value>
		/// The contacts resource.
		/// </value>
		IContacts Contacts { get; }

		/// <summary>
		/// Gets the resource which allows you to notify Zoom that you comply with the policy which requires
		/// you to handle user's data in accordance to the user's preference after the user uninstalls your app.
		/// </summary>
		/// <value>
		/// The data compliance resource.
		/// </value>
		[Obsolete("The Data Compliance API is deprecated")]
		IDataCompliance DataCompliance { get; }

		/// <summary>
		/// Gets the resource which allows you to manage meetings.
		/// </summary>
		/// <value>
		/// The meetings resource.
		/// </value>
		IMeetings Meetings { get; }

		/// <summary>
		/// Gets the resource which allows you to manage meetings that occured in the past.
		/// </summary>
		/// <value>
		/// The past meetings resource.
		/// </value>
		IPastMeetings PastMeetings { get; }

		/// <summary>
		/// Gets the resource which allows you to manage webinars that occured in the past.
		/// </summary>
		/// <value>
		/// The past webinars resource.
		/// </value>
		IPastWebinars PastWebinars { get; }

		/// <summary>
		/// Gets the resource which allows you to manage roles.
		/// </summary>
		/// <value>
		/// The roles resource.
		/// </value>
		IRoles Roles { get; }

		/// <summary>
		/// Gets the resource which allows you to manage users.
		/// </summary>
		/// <value>
		/// The users resource.
		/// </value>
		IUsers Users { get; }

		/// <summary>
		/// Gets the resource which allows you to manage webinars.
		/// </summary>
		/// <value>
		/// The webinars resource.
		/// </value>
		IWebinars Webinars { get; }

		/// <summary>
		/// Gets the resource which allows you to view metrics.
		/// </summary>
		IDashboards Dashboards { get; }

		/// <summary>
		/// Gets the resource which allows you to view reports.
		/// </summary>
		IReports Reports { get; }

		/// <summary>
		/// Gets the resource which allows you to manage call logs.
		/// </summary>
		ICallLogs CallLogs { get; }

		/// <summary>
		/// Gets the resource which allows you to manage chatbot messages.
		/// </summary>
		IChatbot Chatbot { get; }

		/// <summary>
		/// Gets the resource which allows you to access Zoom Phone API endpoints.
		/// </summary>
		IPhone Phone { get; }
		*/
	}
}
