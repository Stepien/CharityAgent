namespace CharityAgent
{
	public class Site
	{
		public Site()
		{

		}

		public Site(string name, string url, string buttonToClick, SelectMethodEnum selectMethod)
		{
			Name = name;
			Url = url;
			ButtonToClick = buttonToClick;
			SelectMethod = selectMethod;

		}

		public string Name { get; set; }
		public string Url { get; set; }
		public string ButtonToClick { get; set; }
		public SelectMethodEnum SelectMethod { get; set; }
	}
}