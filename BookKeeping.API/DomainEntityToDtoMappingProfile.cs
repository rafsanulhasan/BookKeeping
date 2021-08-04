
using AutoMapper;

using BookKeeping.API.DTOs;
using BookKeeping.Domain.Aggregates;

namespace BookKeeping.API
{
	/// <summary>
	/// Maps Domain Entities to Data Transfer Objects
	/// </summary>
	public class DomainEntityToDtoMappingProfile
		: Profile
	{
		/// <summary>
		/// Instantiates the mapping profile
		/// </summary>
		public DomainEntityToDtoMappingProfile()
		{
			CreateMap<TransactionAggregate, IncomeExpenseDto>()
				.ConvertUsing(
					(aggregate, dto) =>
					{
						dto = new IncomeExpenseDto
						{
							Incomes = aggregate.IncomeAmounts,
							CumuliativeIncomes = aggregate.CumuliativeIncomeAmounts,
							Expenses = aggregate.ExpenseAmounts,
							CumuliativeExpenses = aggregate.CumuliativeExpenseAmounts,
							Result = aggregate.ResultAmounts
						};
						return dto;
					}
				);
		}
	}
}
