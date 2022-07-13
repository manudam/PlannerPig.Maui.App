using System;

namespace StudyPlanner.Domain
{
	public class DomainApp
	{
		static DateTime todaysDate;

		public static DateTime TodaysDate
		{
			get
			{
				if (todaysDate != DateTime.MinValue)
					return todaysDate;
				else
					return DateTime.Now;
			}
			set
			{
				todaysDate = value;
			}
		}
	}
}

