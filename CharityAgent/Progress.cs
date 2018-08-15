namespace CharityAgent
{
	public class Progress
	{
		public double ProgressValue { get; set; }
		public double TotalProgress { get; set; }

		public Progress()
		{

		}

		public Progress(double progressValue, double totalProgress)
		{
			ProgressValue = progressValue;
			TotalProgress = totalProgress;
		}
	}

}


