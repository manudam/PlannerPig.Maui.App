using System.Collections.Generic;

namespace StudyPlanner.Domain.Aggregates
{

	public class StudyLeave
	{
		public List<StudyLeaveDay> LeaveDays { get; set; }

		public StudyLeave()
		{
			this.LeaveDays = new List<StudyLeaveDay>();
		}


	}
	
}

