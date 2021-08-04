using BookKeeping.Data.Abstractions;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookKeeping.Data.Entities
{
	public record TransactionFlowEntity
		: DataEntityBase<int>
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public override int Id { get; init; }

		[Required]
		public string Way { get; set; } = string.Empty;

		public ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
	}
}
