using CakeMailNet.Models;
using System;

namespace CakeMailNet
{
	/// <summary>
	/// The delegate invoked when a token is refreshed.
	/// </summary>
	/// <param name="newRefreshToken">The new refresh token.</param>
	/// <param name="newAccessToken">The new access token.</param>
	public delegate void OnTokenRefreshedDelegate(string newRefreshToken, string newAccessToken);

	/// <summary>
	/// Connect using OAuth.
	/// </summary>
	public class OAuthConnectionInfo : IConnectionInfo
	{
		/// <summary>
		/// Gets the usrname.
		/// </summary>
		public string UserName { get; private set; }

		/// <summary>
		/// Gets the password.
		/// </summary>
		public string Password { get; private set; }

		/// <summary>
		/// Gets the refresh token.
		/// </summary>
		public string RefreshToken { get; internal set; }

		/// <summary>
		/// Gets the access token.
		/// </summary>
		public string AccessToken { get; internal set; }

		/// <summary>
		/// Gets the grant type.
		/// </summary>
		public OAuthGrantType GrantType { get; internal set; }

		/// <summary>
		/// Gets the token expiration time.
		/// </summary>
		public DateTime TokenExpiration { get; internal set; }

		/// <summary>
		/// Gets the delegate invoked when the token is refreshed.
		/// </summary>
		public OnTokenRefreshedDelegate OnTokenRefreshed { get; private set; }

		private OAuthConnectionInfo()
		{
			UserName = string.Empty;
			Password = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OAuthConnectionInfo"/> class.
		/// </summary>
		/// <param name="userName">Your user name.</param>
		/// <param name="password">Your password.</param>
		/// <returns>The connection info.</returns>
		public static OAuthConnectionInfo WithUserCredentials(string userName, string password)
		{
			return WithUserCredentials(userName, password, null, null, null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OAuthConnectionInfo"/> class.
		/// </summary>
		/// <param name="userName">Your user name.</param>
		/// <param name="password">Your password.</param>
		/// <param name="onTokenRefreshed">The delegate invoked when a token is issued.</param>
		/// <returns>The connection info.</returns>
		public static OAuthConnectionInfo WithUserCredentials(string userName, string password, OnTokenRefreshedDelegate onTokenRefreshed)
		{
			return WithUserCredentials(userName, password, null, null, onTokenRefreshed);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OAuthConnectionInfo"/> class.
		/// </summary>
		/// <param name="userName">Your user name.</param>
		/// <param name="password">Your password.</param>
		/// <param name="accessToken">A previously issued access token.</param>
		/// <param name="refreshToken">A previously issued refresh token.</param>
		/// <param name="onTokenRefreshed">The delegate invoked when a token is issued.</param>
		/// <returns>The connection info.</returns>
		public static OAuthConnectionInfo WithUserCredentials(string userName, string password, string accessToken, string refreshToken, OnTokenRefreshedDelegate onTokenRefreshed)
		{
			return new OAuthConnectionInfo
			{
				UserName = userName,
				Password = password,
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				GrantType = string.IsNullOrEmpty(refreshToken) ? OAuthGrantType.Password : OAuthGrantType.RefreshToken,
				TokenExpiration = string.IsNullOrEmpty(accessToken) ? DateTime.MinValue : DateTime.MaxValue, // Set expiration to DateTime.MaxValue when an access token is provided because we don't know when it will expire
				OnTokenRefreshed = onTokenRefreshed,
			};
		}
	}
}
