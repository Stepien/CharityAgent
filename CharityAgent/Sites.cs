using System.Collections.Generic;

namespace CharityAgent
{
	public class Sites
	{
		//public Site[] SitesItems { get; set; }

		//public Site GetSiteAtIndex(int index)
		//{
		//	return SitesItems[index];
		//}
		//public void SetSiteAtIndex(Site s, int index)
		//{
		//	SitesItems[index] = s;
		//}

		//public IEnumerator<Site> GetEnumerator()
		//{
		//	foreach (Site s in SitesItems)
		//	{
		//		if (s != null)
		//		{
		//			yield return s;
		//		}
		//	}
		//}

		//IEnumerator IEnumerable.GetEnumerator()
		//{
		//	return GetEnumerator();
		//}


		public static IEnumerable<object[]> SimpleNavigateClickButtonWebsite()
		{
			#region NationalSites
			yield return new object[] { new Site("e-inclusionsite.mondodigitale.org/", "http://e-inclusionsite.mondodigitale.org/", "/html/body/div/div[2]/div[1]/div[1]", SelectMethodEnum.ByXpath) };
			yield return new object[] { new Site("bhookh", "http://www.bhookh.com/", "/html/body/div[1]/table/tbody/tr/td/table/tbody/tr[3]/td/div/img", SelectMethodEnum.ByXpath) };
			yield return new object[] { new Site("creigResarchLabs", "http://www.craigresearchlabs.com/cancer.html", "/html/body/h1/center/table/tbody/tr[2]/td[2]/center[1]/a/img", SelectMethodEnum.ByXpath) };

			#region GreeterGood
			yield return new object[] { new Site("therainforestsite.greatergood.com", "http://therainforestsite.greatergood.com/clickToGive/trs/home", "button-main", SelectMethodEnum.ByClassName) };
			yield return new object[] { new Site("thehungersite.greatergood.com", "http://thehungersite.greatergood.com/clickToGive/ths/home", "button-main", SelectMethodEnum.ByClassName) };
			yield return new object[] { new Site("theliteracysite.greatergood.com", "http://theliteracysite.greatergood.com/clickToGive/lit/home", "button-main", SelectMethodEnum.ByClassName) };
			yield return new object[] { new Site("theanimalrescuesite.greatergood.com", "http://theanimalrescuesite.greatergood.com/clickToGive/trs/home", "button-main", SelectMethodEnum.ByClassName) };
			#endregion
			#region Care2
			yield return new object[] { new Site("care2.com/click-to-donate/violence-against-women/", "http://www.care2.com/click-to-donate/violence-against-women/", "c2dm-click-clickToBtn", SelectMethodEnum.ById) };
			yield return new object[] { new Site("care2.com/click-to-donate/children/", "http://www.care2.com/click-to-donate/children/", "c2dm-click-clickToBtn", SelectMethodEnum.ById) };
			yield return new object[] { new Site("care2.com/click-to-donate/rainforest/", "http://www.care2.com/click-to-donate/rainforest/", "c2dm-click-clickToBtn", SelectMethodEnum.ById) };
			yield return new object[] { new Site("care2.com/click-to-donate/big-cats/", "http://www.care2.com/click-to-donate/big-cats/", "c2dm-click-clickToBtn", SelectMethodEnum.ById) };
			yield return new object[] { new Site("care2.com/click-to-donate/seals/", "http://www.care2.com/click-to-donate/seals/", "c2dm-click-clickToBtn", SelectMethodEnum.ById) };
			yield return new object[] { new Site("care2.com/click-to-donate/oceans/", "http://www.care2.com/click-to-donate/oceans/", "c2dm-click-clickToBtn", SelectMethodEnum.ById) };
			yield return new object[] { new Site("care2.com/click-to-donate/animal-rescue/", "http://www.care2.com/click-to-donate/animal-rescue/", "c2dm-click-clickToBtn", SelectMethodEnum.ById) };
			yield return new object[] { new Site("care2.com/click-to-donate/wolves/", "http://www.care2.com/click-to-donate/wolves/", "c2dm-click-clickToBtn", SelectMethodEnum.ById) };
			#endregion
			#endregion

			#region PolishSites
			yield return new object[] { new Site("pajacyk", "https://www.pajacyk.pl/#index", "paj-click", SelectMethodEnum.ByClassName) };
			yield return new object[] { new Site("polskieserce.pl", "http://polskieserce.pl/", "buttonPomoc", SelectMethodEnum.ByClassName) };
			yield return new object[] { new Site("okruszek.org.pl", "http://www.okruszek.org.pl/", "click-crumb", SelectMethodEnum.ByClassName) };
			#endregion

		}

		public static IEnumerable<object[]> SitesNavigateHoverClickButtonWebsite()
		{
			yield return new object[] { new Site("Pmiska.pl", "http://www.pmiska.pl/", "#content > div.toppies > a > img.b", SelectMethodEnum.ByCssSelector) };
		}
	}
}


