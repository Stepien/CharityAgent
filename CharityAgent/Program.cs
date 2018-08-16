using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static System.Console;

namespace CharityAgent
{
	public class Program
	{
		public static void Main(string[] args)
		{

			WriteLine("-- Charity Agent --");
			WriteLine("How many browser you wanna run? (prefer 1-7)");
			int maxWebDrivers = GetIntFromConsole();
			WriteLine("CharityAgent Start at: " + DateTime.Now.ToShortTimeString() + " using " + maxWebDrivers + " browsers.");
			WriteLine("Click key \"c\" on keyboard to cancel operations.");
			WriteLine();

			using (ProgressBar progressBar = new ProgressBar())
			{
				var task = ProcessAsynchonously(maxWebDrivers, progressBar);
				task.GetAwaiter().GetResult();
			}

		}

		/// <summary>
		/// Main process creating tasks and link them together
		/// </summary>
		/// <param name="maxWebdrivers"></param>
		/// <param name="progressBar"></param>
		/// <returns></returns>
		async static Task ProcessAsynchonously(int maxWebdrivers, ProgressBar progressBar)
		{
			var cts = new CancellationTokenSource();
			var sites = GetSitesToProcess(TypeOfSiteEnum.SimpleGoAndClickButton).SelectMany(x => x.Select(z => (Site)z)).ToArray();

			Progress progress = new Progress(0, sites.Count());

			Task.Run(() =>
			{
				if (ReadKey().KeyChar == 'c')
					cts.Cancel();
			}, cts.Token);

			var inputList = new BufferBlock<Site>(new DataflowBlockOptions { BoundedCapacity = maxWebdrivers, CancellationToken = cts.Token });

			var processSiteBlock = new TransformBlock<Site, Site>(
				site =>
				{
					Site result = SimpleClickCharity(site);
					return result;
				}
				, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxWebdrivers, CancellationToken = cts.Token });
			var progressProgramBlock = new TransformBlock<Site, Site>(
				site =>
				{
					int currentIndex = Array.IndexOf(sites, site);
					progress.ProgressValue = IncrementProgress(currentIndex, progress, progressBar);
					return site;
				}
				, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxWebdrivers, CancellationToken = cts.Token });
			var outputBlock = new ActionBlock<Site>(
				site =>
				{
					int currentIndex = Array.IndexOf(sites, site);
					//.FindIndex(a => a.Equals(site));
					WriteLine("Site " + (double)currentIndex + "/" + sites.Count());
					WriteLine("{0} - Done", site.Name);
				}
				, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxWebdrivers, CancellationToken = cts.Token });

			inputList.LinkTo(processSiteBlock, new DataflowLinkOptions { PropagateCompletion = true });
			processSiteBlock.LinkTo(progressProgramBlock, new DataflowLinkOptions { PropagateCompletion = true });
			progressProgramBlock.LinkTo(outputBlock, new DataflowLinkOptions { PropagateCompletion = true });

			try
			{
				Parallel.For(0, sites.Count(), new ParallelOptions { MaxDegreeOfParallelism = maxWebdrivers, CancellationToken = cts.Token }
				, index =>
				{
					inputList.SendAsync(sites[index]).GetAwaiter().GetResult();
				});

				inputList.Complete();
				await outputBlock.Completion;

				progressBar.Dispose();

				WriteLine("CharityAgent complete all work at " + DateTime.Now.ToShortTimeString() + " :)");
				WriteLine("Press ENTER to exit.");
			}
			catch (OperationCanceledException)
			{
				WriteLine("Operation has been canceled! Press ENTER to exit.");
			}
			ReadLine();

		}

		/// <summary>
		/// Updating progressBar
		/// </summary>
		/// <param name="processingSite"></param>
		/// <param name="progress"></param>
		/// <param name="progressBar"></param>
		/// <returns></returns>
		public static double IncrementProgress(double processingSite, Progress progress, ProgressBar progressBar)
		{
			if (processingSite > progress.ProgressValue)
			{
				progressBar.Report((Math.Round(processingSite / progress.TotalProgress, 2)));
				progress.ProgressValue = processingSite;
			}
			return progress.ProgressValue;

		}

		/// <summary>
		/// Console validator
		/// </summary>
		/// <returns></returns>
		public static int GetIntFromConsole()
		{
			bool valid = false;
			int ReturnInt = 0;
			while (valid != true)
			{
				try
				{
					ReturnInt = ConvertIntFromConsole();
					if (ReturnInt > 0)
					{
						valid = true;
					}
				}
				catch (FormatException)
				{
					Console.WriteLine("Please enter a valid positive whole number!");
				}
			}
			return ReturnInt;

		}

		/// <summary>
		/// Converter input to int
		/// </summary>
		/// <returns></returns>
		public static int ConvertIntFromConsole()
		{
			return Convert.ToInt32(ReadLine());
		}

		/// <summary>
		/// Main method creating webdriver, go to site and click button
		/// </summary>
		/// <param name="site"></param>
		/// <returns></returns>
		public static Site SimpleClickCharity(Site site)
		{
			//Optional settings

			//ChromeDriverService service = ChromeDriverService.CreateDefaultService();
			//service.HideCommandPromptWindow = true;
			//var options = new ChromeOptions();
			//options.AddArgument("headless");
			//IWebDriver webDriver = new ChromeDriver(service, options, new TimeSpan(0,0,30));

			using (IWebDriver webDriver = new ChromeDriver())
			{
				try
				{
					var wait = new WebDriverWait(webDriver, new TimeSpan(0, 0, 30));
					webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
					webDriver.Navigate().GoToUrl(site.Url);

					switch (site.SelectMethod)
					{
						case SelectMethodEnum.ById:
							var elementId = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id(site.ButtonToClick)));
							elementId.Click();
							break;
						case SelectMethodEnum.ByClassName:
							var elementClassName = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName(site.ButtonToClick)));
							elementClassName.Click();
							break;
						case SelectMethodEnum.ByLinkText:
							var elementLinkText = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.LinkText(site.ButtonToClick)));
							elementLinkText.Click();
							break;
						case SelectMethodEnum.ByPartialLinkText:
							var elementPartialLinkText = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.PartialLinkText(site.ButtonToClick)));
							elementPartialLinkText.Click();
							break;
						case SelectMethodEnum.ByCssSelector:
							var elementCssSelector = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(site.ButtonToClick)));
							elementCssSelector.Click();
							break;
						case SelectMethodEnum.ByXpath:
							var elementXpath = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath(site.ButtonToClick)));
							elementXpath.Click();
							break;
						default:
							Console.WriteLine("Wrong Accesing Method");
							break;
					}
					//for plesure
					Thread.Sleep(500);
				}
				catch (NoSuchElementException)
				{
					Console.WriteLine("There was problem with: " + site.Url + " : " + site.ButtonToClick);
				}
				catch (ElementNotVisibleException)
				{
					Console.WriteLine("Agent couldnt find button: " + site.Url + " : " + site.ButtonToClick);
				}
				catch (WebDriverException)
				{
					Console.WriteLine("There was problem with webdriver trying select: " + site.Url + " : " + site.ButtonToClick);
				}
				catch (NullReferenceException)
				{
					Console.WriteLine("Problem with loading site: " + site.Url);
				}
				catch (OperationCanceledException)
				{
					Console.WriteLine("Problem with to many webdrivers");
				}
				finally
				{
					webDriver.Dispose();
				}
			}
			return site;

		}

		/// <summary>
		/// Lists of Charity sites group by "charity clicks" methods
		/// </summary>
		/// <param name="typeOfSite"></param>
		/// <returns></returns>
		public static IEnumerable<object[]> GetSitesToProcess(TypeOfSiteEnum typeOfSite)
		{
			switch (typeOfSite)
			{
				case TypeOfSiteEnum.SimpleGoAndClickButton:
					return Sites.SimpleNavigateClickButtonWebsite();

				case TypeOfSiteEnum.GoHoverClickButton:
					return Sites.SitesNavigateHoverClickButtonWebsite();

				default:
					return null;
			}

		}
	}

}