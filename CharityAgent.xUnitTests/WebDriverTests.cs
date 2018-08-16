using OpenQA.Selenium.Chrome;
using System;
using Xunit;

namespace CharityAgent.Tests
{
	public class WebDriverTests : IDisposable
	{
		ChromeDriver driver;

		public WebDriverTests()
		{
			driver = new ChromeDriver();
		}

		public void Dispose()
		{
			driver.Dispose();
		}


		[Fact]
		[Trait("Category", "SkipWhenLiveUnitTesting")]
		public void Driver_CheckIfDriverWorkingAndRunningGoogle_ShouldReturnGoogle()
		{
			//Arrange
			driver.Navigate().GoToUrl("https://www.google.com/");
			//Act
			var result = driver.Title;
			//Assert
			Assert.Equal("Google", result);
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
