using BookKeeping.Domain.Abstractions;

using static BookKeeping.Domain.Entities.TransactionFlowConstants;

namespace BookKeeping.Domain.Entities
{
	public class Transaction
		: IDomainEntity
	{
		public string Id { get; set; } = string.Empty;
		public string Amount { get; set; } = string.Empty;
		public string TransactionType { get; set; } = string.Empty;
		public TransactionFlows Flow { get; set; }
	}
}
