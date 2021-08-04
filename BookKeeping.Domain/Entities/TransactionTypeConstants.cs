
using System.ComponentModel;

namespace BookKeeping.Domain.Entities
{
	public static class TransactionTypeConstants
	{
		public const string Type1 = "Type 1";
		public const string Type2 = "Type 2";
		public const string Type3 = "Type 3";

		public enum TransactionTypes
		{
			[Description(TransactionTypeConstants.Type1)]
			Type1 = 1,

			[Description(TransactionTypeConstants.Type2)]
			Type2 = 2,

			[Description(TransactionTypeConstants.Type3)]
			Type3 = 3,
		}
	}
}
