using System;
using Newtonsoft.Json;

namespace StudyPlanner.Domain.Aggregates
{

	public class Timetable : BaseModel
	{
		string loginUserId;

		[JsonProperty(PropertyName = "loginUserId")]
		public string LoginUserId
		{
			get
			{
				return loginUserId;
			}
			set
			{
				SetPropertyChanged(ref loginUserId, value);
			}
		}

		string examId;

		[JsonProperty(PropertyName = "examId")]
		public string ExamId
		{
			get
			{
				return examId;
			}
			set
			{
				SetPropertyChanged(ref examId, value);
			}
		}

		int hours;

		[JsonProperty(PropertyName = "hours")]
		public int Hours
		{
			get
			{
				return hours;
			}
			set
			{
				SetPropertyChanged(ref hours, value);
			}
		}

		DayOfWeek day;

		[JsonProperty(PropertyName = "day")]
		public DayOfWeek Day
		{
			get
			{
				return day;
			}
			set
			{
				SetPropertyChanged(ref day, value);
			}
		}

	
	}
	
}

