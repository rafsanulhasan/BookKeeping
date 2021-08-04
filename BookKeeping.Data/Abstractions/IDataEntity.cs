using System;
using System.ComponentModel.DataAnnotations;

namespace BookKeeping.Data.Abstractions
{
	public interface IDataEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		[Key]
		TKey Id { get; }
	}
}
