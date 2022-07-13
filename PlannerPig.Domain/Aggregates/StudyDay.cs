using System;
using Newtonsoft.Json;

namespace StudyPlanner.Domain.Aggregates
{

	public class StudyDay : BaseModel
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

		string planId;

		[JsonProperty(PropertyName = "planId")]
		public string PlanId
		{
			get
			{
				return planId;
			}
			set
			{
				SetPropertyChanged(ref planId, value);
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

		DayOfWeek dayOfTheWeek;

		[JsonProperty(PropertyName = "dayOfTheWeek")]
		public DayOfWeek DayOfTheWeek
		{
			get
			{
				return dayOfTheWeek;
			}
			set
			{
				SetPropertyChanged(ref dayOfTheWeek, value);
			}
		}

		StudyDayType type;

		[JsonProperty(PropertyName = "type")]
		public StudyDayType Type
		{
			get
			{
				return type;
			}
			set
			{
				SetPropertyChanged(ref type, value);
			}
		}

	}
}

