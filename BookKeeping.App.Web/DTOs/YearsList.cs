
using Microsoft.AspNetCore.Mvc.Abstractions;

using System.Collections.Generic;

namespace BookKeeping.API.DTOs
{
	public record YearsList
		: ETaggable
	{
		public List<int> Years { get; set; } = new List<int>();
	}
}
