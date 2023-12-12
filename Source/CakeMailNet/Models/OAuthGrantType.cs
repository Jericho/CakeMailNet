using System.Runtime.Serialization;

namespace CakeMailNet.Models
{
	/// <summary>
	/// Enumeration to indicate the OAuth grant type.
	/// </summary>
	public enum OAuthGrantType
	{
		/// <summary>
		/// Username and password.
		/// </summary>
		[EnumMember(Value = "password")]
		Password,

		/// <summary>
		/// Refresh token.
		/// </summary>
		[EnumMember(Value = "refresh_token")]
		RefreshToken
	}
}
