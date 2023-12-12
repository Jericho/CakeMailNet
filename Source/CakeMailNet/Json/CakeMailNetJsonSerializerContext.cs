using System.Text.Json.Serialization;

namespace CakeMailNet.Json
{
	[JsonSerializable(typeof(System.Text.Json.Nodes.JsonObject))]
	[JsonSerializable(typeof(System.Text.Json.Nodes.JsonObject[]))]

	internal partial class CakeMailNetJsonSerializerContext : JsonSerializerContext
	{
	}
}
