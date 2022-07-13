namespace StudyPlanner.Domain.Aggregates
{
	public class TopicViewModel
	{
		public Topic Topic { get; set; }
		public int HoursLeft { get; set; }
		public int HoursTotal { get; set; }
		public int HoursInFuture { get; set; }
		public int HoursCompleted { get; set; }
		public string Notes { get; set; }
		public double Percentage { get; set; }
		public int PercentageTotal { get; set; }
		public double PercentageRemain { get; set; }
		public int PercentageTotalRemain { get; set; }

	}
	
}

