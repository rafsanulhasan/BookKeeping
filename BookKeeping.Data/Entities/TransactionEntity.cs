using BookKeeping.Data.Abstractions;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookKeeping.Data.Entities
{
	public record TransactionEntity
		: DataEntityBase<int>
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public override int Id { get; init; }

		[Required]
		public DateTime TransactionDate { get; set; } = DateTime.Now;

		[Required]
		public double Amount { get; set; }

		[Required]
		public int TransactionFlowId { get; set; }
		public virtual TransactionFlowEntity TransactionFlow { get; set; } = new TransactionFlowEntity();
		
		[Required]
		public int TransactionTypeId { get; set; }
		public TransactionTypeEntity TransactionType { get; set; } = new TransactionTypeEntity();
		public string Currency { get; set; } = string.Empty;
	}
}
