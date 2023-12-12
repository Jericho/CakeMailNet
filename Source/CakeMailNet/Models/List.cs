using System.Text.Json.Serialization;

namespace CakeMailNet.Models
{
	/// <summary>
	/// List.
	/// </summary>
	public class List
	{
		[JsonPropertyName("id")]
		public long Id { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("status")]
		public string Status { get; set; }

		[JsonPropertyName("language")]
		public string Language { get; set; }
	}
}
