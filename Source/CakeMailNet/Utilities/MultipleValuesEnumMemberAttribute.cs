using System;

namespace CakeMailNet.Utilities
{
	internal class MultipleValuesEnumMemberAttribute : Attribute
	{
		public string DefaultValue { get; set; }

		public string[] OtherValues { get; set; }
	}
}
