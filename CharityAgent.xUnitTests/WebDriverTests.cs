using OpenQA.Selenium.Chrome;
using Xunit;

namespace CharityAgent.xUnitTests
{
	public class WebDriverTests
	{

		[Fact]
		[Trait("Category", "SkipWhenLiveUnitTesting")]
		public void Driver_CheckIfDriverWorkingAndRunningGoogle_ShouldReturnGoogle()
		{
			//Arrange
			var driver = new ChromeDriver();
			driver.Manage().Window.Maximize();
			//Act
			driver.Navigate().GoToUrl("https://www.google.com/");
			//Assert
			Assert.Equal("Google", driver.Title);
			driver.Dispose();
		}

		[Fact]
		[Trait("Category", "SkipWhenLiveUnitTesting")]
		public void SimpleClickCharity_NormalBehavior_WebDriverShouldRunAndReturnSite()
		{
			Site testSite = new Site("pajacyk", "https://www.pajacyk.pl/#index", "paj-click", SelectMethodEnum.ByClassName);

			var result = Program.SimpleClickCharity(testSite);

			Assert.Equal(testSite, result);
		}

	}
}
