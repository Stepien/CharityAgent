using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static System.Console;

namespace CharityAgent
{
	/// <summary>
	/// Simple program that run browsers to clicks on "Charity Buttons". 
	/// Using .Net Core 2.1, Selenium, Producer/Consumer pattern, Parallel Pipeline with TPL DataFlow, xUnit
	/// and progressBar - https://gist.github.com/DanielSWolf/0ab6a96899cc5377bf54
	/// </summary>
	/// 
	/// TODO:
	///		- adding more sites;
	///		- adding more complicated sites;
	///		- adding tests for sites;
	/// 
	public class Program
	{
		public static void Main(string[] args)
		{

			WriteLine("-- Charity Agent --");
			WriteLine("how many browser you wanna run? (prefer 1-7)");
			int maxWebDrivers = GetIntFromConsole();
			WriteLine("CharityAgent Start at: " + DateTime.Now.ToShortTimeString() + " using " + maxWebDrivers + " browsers.");
			WriteLine("Click key \"c\" on keyboard to cancel operations.");
			WriteLine();

			var sites = GetSitesToProcess(TypeOfSiteEnum.SimpleGoAndClickButton);

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
			var sites = GetSitesToProcess(TypeOfSiteEnum.SimpleGoAndClickButton);
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
					WriteLine("Site " + (double)currentIndex + "/" + sites.Count());
					WriteLine("{0} - Done", site.Name);
				}
				, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxWebdrivers, CancellationToken = cts.Token });

			inputList.LinkTo(processSiteBlock, new DataflowLinkOptions { PropagateCompletion = true });
			processSiteBlock.LinkTo(progressProgramBlock, new DataflowLinkOptions { PropagateCompletion = true });
			progressProgramBlock.LinkTo(outputBlock, new DataflowLinkOptions { PropagateCompletion = true });

			try
			{
				Parallel.For(0, sites.Count(), new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = cts.Token }
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
		/// Simple creating webdrover, go to site and click button
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
		public static Site[] GetSitesToProcess(TypeOfSiteEnum typeOfSite)
		{
			// Sites type go to url and click button

			#region NationaSites

			#region GreeterGood
			Site theRainForestSite = new Site("therainforestsite.greatergood.com", "http://therainforestsite.greatergood.com", "button-main", SelectMethodEnum.ByClassName);
			Site theHungerSite = new Site("thehungersite.greatergood.com", "http://thehungersite.greatergood.com/clickToGive/ths/home", "button-main", SelectMethodEnum.ByClassName);
			Site theLiteracySite = new Site("theliteracysite.greatergood.com", "http://theliteracysite.greatergood.com/clickToGive/lit/home", "button-main", SelectMethodEnum.ByClassName);
			Site theAnimalRescueSite = new Site("theanimalrescuesite.greatergood.com", "http://theanimalrescuesite.greatergood.com/clickToGive/ars/home", "button-main", SelectMethodEnum.ByClassName);
			#endregion

			Site eInclusionSite = new Site("e-inclusionsite.mondodigitale.org/", "http://e-inclusionsite.mondodigitale.org/", "/html/body/div/div[2]/div[1]/div[1]", SelectMethodEnum.ByXpath);
			Site bhookh = new Site("bhookh", "http://www.bhookh.com/", "/html/body/div[1]/table/tbody/tr/td/table/tbody/tr[3]/td/div/img", SelectMethodEnum.ByXpath);
			Site craigResarchLabs = new Site("creigResarchLabs", "http://www.craigresearchlabs.com/cancer.html", "/html/body/h1/center/table/tbody/tr[2]/td[2]/center[1]/a/img", SelectMethodEnum.ByXpath);

			#region http://www.care2.com/
			Site care2violenceAgainstWomen = new Site("care2.com/click-to-donate/violence-against-women/", "http://www.care2.com/click-to-donate/violence-against-women/", "c2dm-click-clickToBtn", SelectMethodEnum.ById);
			Site care2violenceChildren = new Site("care2.com/click-to-donate/children/", "http://www.care2.com/click-to-donate/children/", "c2dm-click-clickToBtn", SelectMethodEnum.ById);
			Site care2violenceRainforest = new Site("care2.com/click-to-donate/rainforest/", "http://www.care2.com/click-to-donate/rainforest/", "c2dm-click-clickToBtn", SelectMethodEnum.ById);
			Site care2violenceBigCats = new Site("care2.com/click-to-donate/big-cats/", "http://www.care2.com/click-to-donate/big-cats/", "c2dm-click-clickToBtn", SelectMethodEnum.ById);
			Site care2violenceSeals = new Site("care2.com/click-to-donate/seals/", "http://www.care2.com/click-to-donate/seals/", "c2dm-click-clickToBtn", SelectMethodEnum.ById);
			Site care2violenceOceans = new Site("care2.com/click-to-donate/oceans/", "http://www.care2.com/click-to-donate/oceans/", "c2dm-click-clickToBtn", SelectMethodEnum.ById);
			Site care2violenceAnimalRescue = new Site("care2.com/click-to-donate/animal-rescue/", "http://www.care2.com/click-to-donate/animal-rescue/", "c2dm-click-clickToBtn", SelectMethodEnum.ById);
			Site care2violenceWolves = new Site("care2.com/click-to-donate/wolves/", "http://www.care2.com/click-to-donate/wolves/", "c2dm-click-clickToBtn", SelectMethodEnum.ById);
			#endregion

			#endregion
			#region PolishSites
			Site pajacyk = new Site("pajacyk", "https://www.pajacyk.pl/#index", "paj-click", SelectMethodEnum.ByClassName);
			//Site polskieSerce = new Site("polskieserce.pl", "http://polskieserce.pl/", "buttonPomoc", SelectMethod.ByClassName);
			Site okruszek = new Site("okruszek.org.pl", "http://www.okruszek.org.pl/", "click-crumb", SelectMethodEnum.ByClassName);
			#endregion


			Site[] simpleSitesGoToUrlAndClick = new Site[]
			{
				theRainForestSite,
				theHungerSite,
				bhookh,
				craigResarchLabs,

				care2violenceAgainstWomen,
				care2violenceChildren,
				care2violenceRainforest,
				care2violenceBigCats,
				care2violenceSeals,
				care2violenceOceans,
				care2violenceAnimalRescue,
				care2violenceWolves,

				pajacyk,
				okruszek,
			};

			//Sites type go to url hover on button then click

			Site pmiska = new Site("Pmiska.pl", "http://www.pmiska.pl/", "#content > div.toppies > a > img.b", SelectMethodEnum.ByCssSelector);

			Site[] sitesGoToUrlHoverAndClick = new Site[]
			{
				pmiska
			};

			switch (typeOfSite)
			{
				case TypeOfSiteEnum.SimpleGoAndClickButton:
					return simpleSitesGoToUrlAndClick;

				case TypeOfSiteEnum.GoHoverClickButton:
					return sitesGoToUrlHoverAndClick;

				default:
					return null;
			}

		}
	}

}