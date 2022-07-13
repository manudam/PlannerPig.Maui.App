using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace StudyPlanner.Domain.Aggregates
{
	public class Plan : BaseModel
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

		DateTime startDate;

		[JsonProperty(PropertyName = "startDate")]
		public DateTime StartDate
		{
			get
			{
				return DateTime.SpecifyKind(startDate, DateTimeKind.Unspecified);
			}
			set
			{
				SetPropertyChanged(ref startDate, DateTime.SpecifyKind(value, DateTimeKind.Unspecified));
			}
		}
			
		[JsonIgnore]
		public List<StudyDay> NonRevisionStudyDays { get; set; }

		DateTime revisionStartDate;

		[JsonProperty(PropertyName = "revisionStartDate")]
		public DateTime RevisionStartDate
		{
			get
			{
				return DateTime.SpecifyKind(revisionStartDate, DateTimeKind.Unspecified);
			}
			set
			{
				SetPropertyChanged(ref revisionStartDate, DateTime.SpecifyKind(value, DateTimeKind.Unspecified));
			}
		}

		[JsonIgnore]
		public bool StudyDaysIsDirty
		{
			get
			{
				if (RevisionStudyDays != null)
				{
					foreach (var revisionStudyDay in RevisionStudyDays)
					{
						if (revisionStudyDay.IsDirty)
							return true;
					}
				}
				if (NonRevisionStudyDays != null)
				{
					foreach (var nonRevisionStudyDay in NonRevisionStudyDays)
					{
						if (nonRevisionStudyDay.IsDirty)
							return true;
					}
				}
				return false;
			}
		}

		[JsonIgnore]
		[JsonProperty(PropertyName="revisionStudyDays")]
		public List<StudyDay> RevisionStudyDays { get; set; }

		DateTime generalRevisionStartDate;
		[JsonProperty(PropertyName="generalRevisionStartDate")]
		public DateTime GeneralRevisionStartDate {
			get {
				return DateTime.SpecifyKind(generalRevisionStartDate, DateTimeKind.Unspecified);
			}
			set {
				SetPropertyChanged(ref generalRevisionStartDate, DateTime.SpecifyKind(value, DateTimeKind.Unspecified));
			}
		}

		[JsonIgnore]
		public StudyLeave StudyLeave { get; set; }

		private bool hasChanged = false;

		[JsonIgnore]
		public bool HasChanged
		{
			get
			{
				return hasChanged;
			}
			set
			{
				hasChanged = value;
			}
		}

		public Plan()
		{
			NonRevisionStudyDays = new List<StudyDay> ();
			RevisionStudyDays = new List<StudyDay> ();
			StudyLeave = new StudyLeave();
		}
	
		public int GetRevHoursByDay(DayOfWeek day)
		{
			foreach (var revisionDay in RevisionStudyDays)
			{
				if (revisionDay.DayOfTheWeek == day)
				{
					return revisionDay.Hours;
				}
			}
			
			return 0;
		}
			
		public void SetRevHoursByDay(int hours, DayOfWeek day)
		{
			foreach (var studyDay in RevisionStudyDays)
			{
				if (studyDay.DayOfTheWeek == day)
				{
					if (studyDay.Hours != hours) {
						studyDay.Hours = hours;
						hasChanged = true;
					}
				}
			}
		}
			
		public int GetNonRevHoursByDay(DayOfWeek day)
		{
			foreach (var studyDay in NonRevisionStudyDays)
			{
				if (studyDay.DayOfTheWeek == day)
				{
					return studyDay.Hours;
				}
			}

			return 0;
		}
			
		public void SetNonRevHoursByDay(int hours, DayOfWeek day)
		{
			foreach (var studyDay in NonRevisionStudyDays)
			{
				if (studyDay.DayOfTheWeek == day)
				{
					if (studyDay.Hours != hours) {
						studyDay.Hours = hours;
						hasChanged = true;
					}
				}
			}
		}
			
		public int GetHoursByDay(DateTime currentDate)
		{
			if (currentDate.Date < this.RevisionStartDate) {
				return GetNonRevHoursByDay (currentDate.DayOfWeek);
			} else if (currentDate.Date < this.GeneralRevisionStartDate.Date) {
				return GetRevHoursByDay (currentDate.DayOfWeek);
			} else {
				return 10;
			}
		}
	

			
		public int TotalRevisionHours(DateTime revisionstartDate, 
									  DateTime revisionEndDate,
									  StudyLeave studyLeave)
		{
			int totalRevisionTime = 0;
			DateTime revisionDate = revisionstartDate;
			DateTime revisionEnd = revisionEndDate;

			while (revisionDate < revisionEnd)
			{
				var studyLeaveDay = studyLeave.LeaveDays.Where(d => d.Date.Date == revisionDate.Date).FirstOrDefault();

				if (studyLeaveDay == null)
				{
					totalRevisionTime += GetRevHoursByDay(revisionDate.DayOfWeek);
				}
				else
				{
					totalRevisionTime += studyLeaveDay.Hours;
				}

				revisionDate = revisionDate.AddDays (1);
			}
			return totalRevisionTime;
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		public void Reset()
		{
			this.StartDate = DomainApp.TodaysDate;

			this.NonRevisionStudyDays = new List<StudyDay>();
			this.NonRevisionStudyDays.Add(new StudyDay{ Hours = 2, DayOfTheWeek = DayOfWeek.Monday });
			this.NonRevisionStudyDays.Add(new StudyDay{ Hours = 2, DayOfTheWeek = DayOfWeek.Tuesday });
			this.NonRevisionStudyDays.Add(new StudyDay{ Hours = 2, DayOfTheWeek = DayOfWeek.Wednesday });
			this.NonRevisionStudyDays.Add(new StudyDay{ Hours = 2, DayOfTheWeek = DayOfWeek.Thursday });
			this.NonRevisionStudyDays.Add(new StudyDay{ Hours = 2, DayOfTheWeek = DayOfWeek.Friday });
			this.NonRevisionStudyDays.Add(new StudyDay{ Hours = 5, DayOfTheWeek = DayOfWeek.Saturday });
			this.NonRevisionStudyDays.Add(new StudyDay{ Hours = 5, DayOfTheWeek = DayOfWeek.Sunday });

			this.RevisionStudyDays = new List<StudyDay>();
			this.RevisionStudyDays.Add(new StudyDay{ Hours = 3, DayOfTheWeek = DayOfWeek.Monday });
			this.RevisionStudyDays.Add(new StudyDay{ Hours = 3, DayOfTheWeek = DayOfWeek.Tuesday });
			this.RevisionStudyDays.Add(new StudyDay{ Hours = 3, DayOfTheWeek = DayOfWeek.Wednesday });
			this.RevisionStudyDays.Add(new StudyDay{ Hours = 3, DayOfTheWeek = DayOfWeek.Thursday });
			this.RevisionStudyDays.Add(new StudyDay{ Hours = 3, DayOfTheWeek = DayOfWeek.Friday });
			this.RevisionStudyDays.Add(new StudyDay{ Hours = 8, DayOfTheWeek = DayOfWeek.Saturday });
			this.RevisionStudyDays.Add(new StudyDay{ Hours = 8, DayOfTheWeek = DayOfWeek.Sunday });


		}
	}
	
	
}	

