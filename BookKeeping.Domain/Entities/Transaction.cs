using BookKeeping.Domain.Abstractions;

using static BookKeeping.Domain.Entities.TransactionFlowConstants;
using static BookKeeping.Domain.Entities.TransactionTypeConstants;

namespace BookKeeping.Domain.Entities
{
	public class Transaction
		: IDomainEntity
	{
		public string Id { get; set; } = string.Empty;
		public string Amount { get; set; } = string.Empty;
		public TransactionTypes TransactionType { get; set; }
		public TransactionFlows Flow { get; set; }
	}
}
