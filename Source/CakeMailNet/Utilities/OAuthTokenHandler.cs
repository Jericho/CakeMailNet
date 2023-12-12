using CakeMailNet.Models;
using Pathoschild.Http.Client;
using Pathoschild.Http.Client.Extensibility;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace CakeMailNet.Utilities
{
	/// <summary>
	/// Handler to ensure requests to the CakeMail next-gen API include a valid OAuth token.
	/// </summary>
	/// <seealso cref="Pathoschild.Http.Client.Extensibility.IHttpFilter" />
	internal class OAuthTokenHandler : IHttpFilter, ITokenHandler
	{
		public string Token
		{
			get
			{
				RefreshTokenIfNecessary(false);
				return _connectionInfo.AccessToken;
			}
		}

		public IConnectionInfo ConnectionInfo
		{
			get => _connectionInfo;
		}

		private static readonly ReaderWriterLockSlim _lock = new();

		private readonly OAuthConnectionInfo _connectionInfo;
		private readonly HttpClient _httpClient;
		private readonly TimeSpan _clockSkew;

		public OAuthTokenHandler(OAuthConnectionInfo connectionInfo, HttpClient httpClient, TimeSpan? clockSkew = null)
		{
			if (connectionInfo == null) throw new ArgumentNullException(nameof(connectionInfo));
			if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

			_connectionInfo = connectionInfo;
			_httpClient = httpClient;
			_clockSkew = clockSkew.GetValueOrDefault(TimeSpan.FromMinutes(5));
		}

		/// <summary>Method invoked just before the HTTP request is submitted. This method can modify the outgoing HTTP request.</summary>
		/// <param name="request">The HTTP request.</param>
		public void OnRequest(IRequest request)
		{
			request.WithBearerAuthentication(Token);
		}

		/// <summary>Method invoked just after the HTTP response is received. This method can modify the incoming HTTP response.</summary>
		/// <param name="response">The HTTP response.</param>
		/// <param name="httpErrorAsException">Whether HTTP error responses should be raised as exceptions.</param>
		public void OnResponse(IResponse response, bool httpErrorAsException) { }

		public string RefreshTokenIfNecessary(bool forceRefresh)
		{
			try
			{
				_lock.EnterUpgradeableReadLock();

				if (forceRefresh || TokenIsExpired())
				{
					try
					{
						_lock.EnterWriteLock();

						var contentValues = new Dictionary<string, string>()
						{
							{ "grant_type", _connectionInfo.GrantType.ToEnumString() },
						};

						switch (_connectionInfo.GrantType)
						{
							case OAuthGrantType.Password:
								contentValues.Add("username", _connectionInfo.UserName);
								contentValues.Add("password", _connectionInfo.Password);
								//contentValues.Add("account_id", ??); // <-- to do: add accountId to connectionInfo ->>
								contentValues.Add("scopes", "user");
								contentValues.Add("expiration_seconds", "3600"); // one hour
								break;
							case OAuthGrantType.RefreshToken:
								if (_connectionInfo.RefreshToken == null) throw new ArgumentNullException("refresh_token", "The refresh token is null");
								contentValues.Add("refresh_token", _connectionInfo.RefreshToken);
								break;
						}

						var requestTime = DateTime.UtcNow;
						var request = new HttpRequestMessage(HttpMethod.Post, "https://api.cakemail.dev/token");
						//request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_connectionInfo.UserName}:{_connectionInfo.Password}")));
						request.Content = new FormUrlEncodedContent(contentValues);
						var response = _httpClient.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
						var responseContent = response.Content.ReadAsStringAsync(null).ConfigureAwait(false).GetAwaiter().GetResult();

						if (string.IsNullOrEmpty(responseContent)) throw new Exception(response.ReasonPhrase);

						var jsonResponse = JsonDocument.Parse(responseContent).RootElement;

						if (!response.IsSuccessStatusCode)
						{
							var errorInfo = response.GetErrorMessageAsync().GetAwaiter().GetResult();
							throw new CakeMailException(errorInfo.Message, response, "No diagnostic available", null);
						}

						//_connectionInfo.RefreshToken = jsonResponse.GetPropertyValue("refresh_token", string.Empty);
						//_connectionInfo.AccessToken = jsonResponse.GetPropertyValue("access_token", string.Empty);
						//_connectionInfo.TokenExpiration = requestTime.AddSeconds(jsonResponse.GetPropertyValue("expires_in", 60 * 60));
						//_connectionInfo.TokenScope = new ReadOnlyDictionary<string, string[]>(
						//	jsonResponse.GetPropertyValue("scope", string.Empty)
						//		.Split(' ')
						//		.Select(x => x.Split(new[] { ':' }, 2))
						//		.Select(x => new KeyValuePair<string, string[]>(x[0], x.Skip(1).ToArray()))
						//		.GroupBy(x => x.Key)
						//		.ToDictionary(
						//			x => x.Key,
						//			x => x.SelectMany(c => c.Value).ToArray()));

						//// Please note that Server-to-Server OAuth does not use the refresh token.
						//// Therefore change the grant type to 'RefreshToken' only when the response includes a refresh token.
						//if (!string.IsNullOrEmpty(_connectionInfo.RefreshToken)) _connectionInfo.GrantType = OAuthGrantType.RefreshToken;

						//_connectionInfo.OnTokenRefreshed?.Invoke(_connectionInfo.RefreshToken, _connectionInfo.AccessToken);
					}
					finally
					{
						if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
					}
				}
			}
			finally
			{
				if (_lock.IsUpgradeableReadLockHeld) _lock.ExitUpgradeableReadLock();
			}

			return _connectionInfo.AccessToken;
		}

		private bool TokenIsExpired()
		{
			return _connectionInfo.TokenExpiration <= DateTime.UtcNow.Add(_clockSkew);
		}
	}
}
