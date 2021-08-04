using AutoMapper;

using BookKeeping.API.DTOs;
using BookKeeping.Domain.Aggregates;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookKeeping.API.Controllers
{
	/// <summary>
	/// Works with Transactionsa
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	[ApiVersion("1.0")]
	public class TransactionsController 
		: ControllerBase
	{
		private readonly ITransactionAggregate _aggregate;
		private readonly IMapper _mapper;

		/// <summary>
		/// Instantiates the Api Contoller
		/// </summary>
		/// <param name="aggregate"></param>
		/// <param name="mapper"></param>
		public TransactionsController(
			ITransactionAggregate aggregate,
			IMapper mapper
		)
		{
			_aggregate = aggregate;
			_mapper = mapper;
		}

		/// <summary>
		/// Get all transactions specified by year
		/// </summary>
		/// <param name="year">The year to get transactions on</param>
		/// <returns>
		/// <seealso cref="IncomeExpenseDto">IncomeExpenseDto</seealso>
		/// </returns>
		[HttpGet("{year:int}")]
		public async Task<IncomeExpenseDto> Get(int year)
		{
			await _aggregate.GetTransactionsAsync(year);
			var dto = _mapper.Map<IncomeExpenseDto>(_aggregate);
			return dto;
		}

		[Route("years")]
		[HttpGet]
		public Task<ICollection<int>> GetYears()
		{
			return _aggregate.GetYearsAsync();
		}
	}
}
