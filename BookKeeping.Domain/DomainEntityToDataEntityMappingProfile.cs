using System;

using AutoMapper;

using BookKeeping.Data.Entities;
using BookKeeping.Domain.Entities;

using static BookKeeping.Domain.Entities.TransactionFlowConstants;
using static BookKeeping.Domain.Entities.TransactionTypeConstants;

namespace BookKeeping.Domain
{
	public class DomainEntityToDataEntityMappingProfile
		: Profile
	{
		public DomainEntityToDataEntityMappingProfile()
		{
			CreateMap<TransactionEntity, Transaction>()
				.ConvertUsing(
					(tEntity, t) =>
					{
						t = new Transaction
						{
							Id = tEntity.Id.ToString(),
							Amount = $"${tEntity.Currency}{tEntity.Amount}"
						};
						if (Enum.TryParse<TransactionFlows>(tEntity.TransactionFlow.Id.ToString(), out var flow))
						{
							t.Flow = flow;
						}
						if (Enum.TryParse<TransactionTypes>(tEntity.TransactionType.Id.ToString(), out var type))
						{
							t.TransactionType = type;
						}

						return t;
					}
				);
		}
	}
}
