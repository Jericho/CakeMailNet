using Microsoft.IO;

namespace CakeMailNet.Utilities
{
	/// <summary>
	/// Utils.
	/// </summary>
	internal static class Utils
	{
		public static RecyclableMemoryStreamManager MemoryStreamManager { get; } = new RecyclableMemoryStreamManager();
	}
}
