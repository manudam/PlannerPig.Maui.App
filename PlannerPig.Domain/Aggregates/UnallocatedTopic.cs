using System;
using StudyPlanner.Resources;

namespace StudyPlanner.Domain.Aggregates
{
	
	public class UnallocatedTopic
	{
		public string TopicName { get; set; }
		public string ExamName { get; set; }
		public EventType EventType { get; set; }

		public string Description {
			get {
				return string.Format (
					Interface.PlannerPage_ValidationTopicsNotAllocatedItem,
					TopicName,
					ExamName,
					EventType,
					Environment.NewLine);
			}
		}
	}
	

}

