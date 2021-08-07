using Fluxor;

using System;

namespace BookKeeping.App.Web.Store
{
	public class Features
	{
		public class IncomeExpenseFeature
			: Feature<IncomeExpenseState>
		{
			private const string IncomeExpenseFeatureName = "IncomeExpense";
			public override string GetName()
				=> IncomeExpenseFeatureName;

			protected override IncomeExpenseState GetInitialState()
				=> new(
					false, 
					false,
					false,
					new(),
					TimeSpan.FromMinutes(1),
					null,
					null,
					null
				);
		}

		public class FetchYearsFeature
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
					null,
					null
				);
		}
		public class FetchIncomeExpenseFeature
		    : Feature<SelectedYearState>
		{
			private const string IncomeExpenseFeatureName = "FetchIncomeExpense";
			public override string GetName()
				=> IncomeExpenseFeatureName;

			protected override SelectedYearState GetInitialState()
				=> new(0);
		}
	}
}
