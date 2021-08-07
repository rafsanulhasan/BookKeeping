using AutoMapper;

using BookKeeping.API.DTOs;
using BookKeeping.Domain.Aggregates;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookKeeping.API.Controllers
{
	/// <summary>
	/// Works with Transactionsa
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	//[ApiVersion("1.0")]
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
		//[ResponseCache(Duration = 60)]
		public async Task<ActionResult<IncomeExpenseDto>> Get(int year)
		{
			try
			{
				await _aggregate.GetTransactionsAsync(year);
				var dto = _mapper.Map<IncomeExpenseDto>(_aggregate);
				return Ok(dto);
			}
			catch (Exception ex)
			{
				return Ok(new
				{
					error = ex.Message
				});
			}
		}

		[Route("years")]
		[HttpGet]
		//[ResponseCache(Duration = 60)]
		public async Task<ActionResult<YearsList>> GetYears()
		//public async Task<ActionResult<YearsList>> GetYears()
		{
			var years = await _aggregate.GetYearsAsync();
			var yearList = new YearsList { Years = years.ToList() };
			//return Ok(yearList);
			return Ok(yearList);
		}
	}
}
