using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace StudyPlanner.Domain.Aggregates
{
	public class Exam : BaseModel, IComparable
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

		string name;

		[JsonProperty(PropertyName = "name")]
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				SetPropertyChanged(ref name, value);
			}
		}

		DateTime examDate;

		[JsonProperty(PropertyName = "examDate")]
		public DateTime ExamDate
		{
			get
			{
				return DateTime.SpecifyKind(examDate, DateTimeKind.Unspecified);
			}
			set
			{
				SetPropertyChanged(ref examDate, DateTime.SpecifyKind(value, DateTimeKind.Unspecified));
			}
		}

		DateTime studyStartDate;

		[JsonProperty(PropertyName = "studyStartDate")]
		public DateTime StudyStartDate
		{
			get
			{
				return DateTime.SpecifyKind(studyStartDate, DateTimeKind.Unspecified);
			}
			set
			{
				SetPropertyChanged(ref studyStartDate, DateTime.SpecifyKind(value, DateTimeKind.Unspecified));
			}
		}

		int regroupDays;

		[JsonProperty(PropertyName = "regroupDays")]
		public int RegroupDays
		{
			get
			{
				return regroupDays;
			}
			set
			{
				SetPropertyChanged(ref regroupDays, value);
			}
		}

		string colourHex;

		[JsonProperty(PropertyName = "colourHex")]
		public string ColourHex
		{
			get
			{
				return colourHex;
			}
			set
			{
				SetPropertyChanged(ref colourHex, value);
			}
		}

		double weight;

		[JsonProperty(PropertyName = "weight")]
		public double Weight
		{
			get
			{
				return weight;
			}
			set
			{
				SetPropertyChanged(ref weight, value);
			}
		}

		[JsonIgnore]
		public TopicList Topics { get; set; }

		[JsonIgnore]
		public bool TopicsDirty
		{
			get
			{
				if (Topics != null)
				{
					foreach (var topic in Topics)
					{
						if (topic.IsDirty)
							return true;
					}
				}
				return false;
			}
		}

		[JsonIgnore]
		public List<Timetable> Timetables { get; set; }

        [JsonIgnore]
        public bool TimetableHasChanged 
        {
            get
            {
                bool timetableHasChanged = false;
                if (Timetables != null)
                {
                    foreach (var timetable in Timetables)
                    {
                        if (timetable.IsDirty)
                        {
                            return true;
                        }
                    }
                }
                return timetableHasChanged;
            }
        }

		[JsonIgnore]
		public List<TimetableException> TimetableExceptions { get; set; }

        [JsonIgnore]
        public bool IsNewExam { get; set; }

        [JsonIgnore]
        public bool WizardShown { get; set; }

        public Exam()
		{
			Topics = new TopicList();
			Timetables = new List<Timetable>();
			TimetableExceptions = new List<TimetableException>();
		}
			
		public int CompareTo (object obj)
		{
			Exam examToCompareTo = (Exam) obj;
			
			return this.ExamDate.CompareTo(examToCompareTo.ExamDate);
		}

		public int DaysTillExam()
		{
			return Convert.ToInt32 (ExamDate.Subtract (DomainApp.TodaysDate.Date).TotalDays);
		}

		public int TotalStudyHours(DateTime studystartDate,
								   DateTime studyEndDate)
		{
			int totalStudyTime = 0;
			DateTime studyDate = studystartDate.Date;
			DateTime studyEnd = studyEndDate.Date;

			while (studyDate < studyEnd)
			{
				var timetableException = TimetableExceptions
					.FirstOrDefault(d => (d.Date.Date == studyDate.Date));

				if (timetableException == null)
				{
					var timetable = Timetables
					.FirstOrDefault(d => d.Day == studyDate.DayOfWeek);
					
					totalStudyTime += timetable != null ? timetable.Hours : 0;
				}
				else
				{
					totalStudyTime += timetableException.Hours;
				}

				studyDate = studyDate.AddDays(1);
			}
			return totalStudyTime;
		}

        public DateTime ActualStudyStartDate()
        {
			DateTime studyDate = StudyStartDate.Date.AddDays(-1);

			if (DomainApp.TodaysDate.Date.AddDays(-1) > studyDate.Date)
			{
				studyDate = DomainApp.TodaysDate.Date.AddDays(-1);
			}

            return studyDate;
        }

public DateTime ActualStudyEndDate()
		{
			var generalRevisionStartDate = ExamDate.Date.AddDays(-1 * RegroupDays);

            return generalRevisionStartDate.AddDays(-1);
		}
		
	}
	
	public class ExamList : List<Exam>
	{
		public bool IsExamDate(DateTime date)
		{
			var isExamDate = false;

       		foreach (var exam in this)
			{
				if (exam.ExamDate.ToString("d") == date.ToString("d"))
				{
					isExamDate = true;
					break;
				}
			}
			
        	return isExamDate;
		}
       
		public DateTime? LastExamDate()
		{
			DateTime? lastExamDate = null;
		
			if (this.Count > 0)
			{
				lastExamDate = this[0].ExamDate.Date;
				
				foreach (var exam in this)
				{
					if (exam.ExamDate.Date > lastExamDate)
					{
						lastExamDate = exam.ExamDate.Date;	
					}
				}
			}

			
			return lastExamDate;
				
		}
		
		public DateTime? FirstExamDate()
		{
			DateTime? firstExamDate = null;

			if (this.Count > 0) {
				
				firstExamDate = this [0].ExamDate;
			
				foreach (var exam in this) {
					if (exam.ExamDate < firstExamDate) {
						firstExamDate = exam.ExamDate;	
					}
				}
			} 

			return firstExamDate;
			
		}

		public DateTime? FirstStudyStartDate()
		{
			DateTime? firstStudyStartDate = null;

			if (this.Count > 0)
			{
				firstStudyStartDate = this[0].StudyStartDate;

				foreach (var exam in this)
				{
					if (exam.StudyStartDate < firstStudyStartDate)
					{
						firstStudyStartDate = exam.StudyStartDate;
					}
				}
			}

			return firstStudyStartDate;
		}

		public int DaysTillFirstExam(DateTime dateToCompare)
		{
			var firstExamDate = FirstExamDate ();

			if (firstExamDate.HasValue) {
				return Convert.ToInt32 (firstExamDate.Value.Date.Subtract (dateToCompare.Date).TotalDays);
			} 

			return 0;
		}
		
		public Exam FirstExam()
		{
			Exam firstExam = null;

			if (this.Count > 0)
			{
				var firstExamDate = this[0].ExamDate;
				firstExam = (Exam) this[0];
				
				foreach (var exam in this)
				{
					if (exam.ExamDate < firstExamDate)
					{
						firstExamDate = exam.ExamDate.Date;	
						firstExam = exam;
					}
				}
			}
					
			return firstExam;
		}
		
		public DateTime PreviousExamDate(Exam exam)
		{
			int index = this.IndexOf(exam);

			if (index != 0)
			{
				return this[index - 1].ExamDate;
			}
			else
			{
				return DateTime.MinValue;
			}
		}

		private int RealDaysInBetween(DateTime startDate, 
		                              DateTime endDate,
		                              Plan plan)
		{
			int realDays = 0;
			
			while (startDate < endDate)
			{
				if (plan.GetRevHoursByDay(startDate.DayOfWeek) != 0)
				{
					realDays += 1;
				}
				
				startDate = startDate.AddDays(1);
			}

			return realDays;
			
		}

		public double WeightForExam(DateTime date,
			string examId)
		{
			int examsThatCount = 0;

			Exam exam = this.FirstOrDefault (e => e.Id == examId);

			//check if the exam counts for this date
			if (date.Date > exam.ExamDate.Date)
			{
				return 0;
			}

			//check how many exams count for this date
			foreach(var e in this)
			{
				if (date.Date < e.ExamDate.Date)
				{
					examsThatCount += 1;
				}
			}

			return (Convert.ToDouble(1) / examsThatCount);
		}

		public string PickNextColour(List<string> colours)
		{
			foreach (var colour in colours)
			{
				if (!this.Select(p => p.ColourHex).Contains(colour))
				{
					return colour;
				}
			}

			return colours[0];
		}
		

       
	}
}

