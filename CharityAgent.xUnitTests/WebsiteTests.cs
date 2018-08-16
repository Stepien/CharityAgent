using OpenQA.Selenium.Chrome;
using System;
using Xunit;

namespace CharityAgent.Tests
{
	public class WebsiteTests : IDisposable
	{
		ChromeDriver driver;

		public WebsiteTests()
		{
			driver = new ChromeDriver();
		}

		public void Dispose()
		{
			driver.Dispose();
		}


		[Theory]
		[Trait("Category", "SkipWhenLiveUnitTesting")]
		[MemberData(nameof(Sites.SimpleNavigateClickButtonWebsite), MemberType = typeof(Sites))]
		public void WebsitesSimpleNavigateClickButonWebsite_CheckIfWebsiteIsUp_ShouldReturnExpectedUrl(Site site)
		{
			try
			{
				//Arrange

				//Act
				driver.Navigate().GoToUrl(site.Url);
				//Assert
				Assert.Equal(site.Url, driver.Url);
			}
			finally
			{
				driver.Dispose();
			}
		}

		[Theory]
		[Trait("Category", "SkipWhenLiveUnitTesting")]
		[MemberData(nameof(Sites.SitesNavigateHoverClickButtonWebsite), MemberType = typeof(Sites))]
		public void SitesNavigateHoverClickButtonWebsite_CheckIfWebsiteIsUp_ShouldReturnExpectedUrl(Site site)
		{
			try
			{
				//Arrange

				//Act
				driver.Navigate().GoToUrl(site.Url);
				//Assert
				Assert.Equal(site.Url, driver.Url);
			}
			finally
			{
				driver.Dispose();
			}
		}
	}
}


