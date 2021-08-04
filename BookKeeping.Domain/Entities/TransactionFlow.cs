
using System.ComponentModel;

namespace BookKeeping.Domain.Entities
{
	public static class TransactionFlowConstants
	{
		public const string Income = "income";
		public const string Expense = "expense";

		public enum TransactionFlows
		{
			[Description(TransactionFlowConstants.Income)]
			Income = 1,

			[Description(TransactionFlowConstants.Expense)]
			Expense = 2
		}
	}
}
