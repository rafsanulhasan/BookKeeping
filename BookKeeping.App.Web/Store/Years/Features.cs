using Fluxor;

using System;

namespace BookKeeping.App.Web.Store.Years
{
	public class YearsFeature
			: Feature<YearsState>
	{
		private const string IncomeExpenseFeatureName = "Years";
		public override string GetName()
			=> IncomeExpenseFeatureName;

		protected override YearsState GetInitialState()
			=> new(
				true,
				false,
				false,
				new(),
				TimeSpan.FromMinutes(1),
				null,
				null
			);
	}
}
