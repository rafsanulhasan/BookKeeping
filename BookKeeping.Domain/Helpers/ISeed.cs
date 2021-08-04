namespace BookKeeping.Domain.Helpers
{
	public interface ISeed
	{
		void Migrate();
		void SeedData();
	}
}