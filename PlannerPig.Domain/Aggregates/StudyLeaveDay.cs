using System;
using Newtonsoft.Json;

namespace StudyPlanner.Domain.Aggregates
{

	public class StudyLeaveDay : BaseModel
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

		DateTime date;
		[JsonProperty(PropertyName="date")]
		public DateTime Date {
			get {
				return DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
			}
			set {
				SetPropertyChanged(ref date, DateTime.SpecifyKind(value, DateTimeKind.Unspecified));
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


	}
	
}

