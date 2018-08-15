using Xunit;

namespace CharityAgent.Tests
{
	public class LogicTests
	{
		[Fact]
		public void IncrementProgress_EqualIncrement_ShouldReturn20()
		{
			var processingSite = 20;
			var progress = new Progress(20, 100);
			var progressBar = new ProgressBar();

			var result = Program.IncrementProgress(processingSite, progress, progressBar);

			Assert.Equal(20, result);

		}

		[Fact]
		public void IncrementProgress_TryingUnderIncrement_ShouldReturn20()
		{
			var processingSite = 10;
			var progress = new Progress(20, 100);
			var progressBar = new ProgressBar();

			var result = Program.IncrementProgress(processingSite, progress, progressBar);

			Assert.Equal(20, result);

		}

		[Fact]
		public void IncrementProgress_NormalIncrement_ShouldReturn30()
		{
			var processingSite = 30;
			var progress = new Progress(20, 100);
			var progressBar = new ProgressBar();

			var result = Program.IncrementProgress(processingSite, progress, progressBar);

			Assert.Equal(30, result);

		}
	}
}
