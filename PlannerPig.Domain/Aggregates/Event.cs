using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using StudyPlanner.Resources;

namespace StudyPlanner.Domain.Aggregates
{
	public class Event : BaseModel
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

		[JsonIgnore]
		public DateTime ExamDate { get; set; }

		string topicId;

		[JsonProperty(PropertyName = "topicId")]
		public string TopicId
		{
			get
			{
				return topicId;
			}
			set
			{
				SetPropertyChanged(ref topicId, value);
			}
		}

		string title;

		[JsonProperty(PropertyName = "title")]
		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				SetPropertyChanged(ref title, value);
			}
		}

		string examName;

		[JsonProperty(PropertyName = "examName")]
		public string ExamName
		{
			get
			{
				return examName;
			}
			set
			{
				SetPropertyChanged(ref examName, value);
			}
		}

		EventType eventType;

		[JsonProperty(PropertyName = "eventType")]
		public EventType EventType
		{
			get
			{
				return eventType;
			}
			set
			{
				SetPropertyChanged(ref eventType, value);
			}
		}

		private DateTime date;
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

        int hoursComplete;

		[JsonProperty(PropertyName = "HoursComplete")]
		public int HoursComplete
		{
			get
			{
				return hoursComplete;
			}
			set
			{
				SetPropertyChanged(ref hoursComplete, value);
			}
		}

        [JsonIgnore]
		public int HoursLeft
		{
			get
			{
                return Hours - HoursComplete;
			}
		}

		bool complete;

		[JsonProperty(PropertyName = "complete")]
		public bool Complete
		{
			get
			{
				return complete;
			}
			set
			{
				SetPropertyChanged(ref complete, value);
			}
		}
		
		bool partiallyComplete;

		[JsonProperty(PropertyName = "partiallyComplete")]
		public bool PartiallyComplete
		{
			get
			{
				return partiallyComplete;
			}
			set
			{
				SetPropertyChanged(ref partiallyComplete, value);
			}
		}


		[JsonIgnore]
		public int SortOrder { get; set; }

		[JsonIgnore]
		public string EventHours
		{
			get{
				return string.Format (Interface.Event_Hours, Hours);
			}

		}

	}

	public class EventList : ObservableCollection<Event>
	{

		public DateTime GetLastEventDay
		{
			get 
			{
				return this.ToList().Last().Date;
			}
		}

		public DateTime GetFirstEventDay
		{
			get 
			{
				return this.ToList().First().Date;
			}
		}

		public List<Event> GetEventByDate(DateTime eventDate)
		{
			var thisEvent = from e in this.ToList()
							where e.Date.Date == eventDate.Date
							select e;
					
			return thisEvent
				.ToList();
		}

		public int StudyHoursPlannedByDate(DateTime eventDate)
		{
			return this.ToList()
				.Where(p => p.Date.Date == eventDate.Date)
				.Where(p => p.EventType == EventType.Study)
				.Sum(p => p.Hours);
		}

		public int StudyHoursPlannedByDateRange(DateTime startEventDate, DateTime endEventDate)
		{
			return this.ToList()
				.Where(p => p.Date.Date >= startEventDate && p.Date.Date <= endEventDate.Date)
				.Where(p => p.EventType == EventType.Study)
				.Sum(p => p.Hours);
		}

		public Event GetLastEventDayByType(EventType eventType)
		{
			if (this.Any (p => p.EventType == eventType)) {
				return this.Last (p => p.EventType == eventType);
			} else {
				return null;
			} 
		}

		public EventType EventTypeByDate(DateTime eventDate)
		{
			var thisEvent = (from e in this.ToList()
				where e.Date.ToString("d") == eventDate.ToString("d")
					select e).FirstOrDefault();
			
			if (thisEvent != null)
			{
				return thisEvent.EventType;
			}
			else
			{
				return EventType.None;
			}
		}

		public int HoursComplete()
		{
			return (from e in this.ToList()
			        where e.Complete == true
			        where e.EventType != EventType.Lost
			        select e.Hours).Sum();
		}
		
		public int HoursLost()
		{
			return (from e in this.ToList()
			        where e.EventType == EventType.Lost     
			        select e.Hours).Sum();
		}
		
		public int HoursComplete(DateTime studystartDate, 
		                         DateTime studyEndDate)
		{
			return (from e in this.ToList()
					where (e.Complete == true)
			    	where e.Date.Date > studystartDate.Date
			       	where e.Date.Date < studyEndDate.Date
	              	select e.Hours).Sum();
		}

		public int HoursLost(DateTime studystartDate, 
		                     DateTime studyEndDate)
		{
			return (from e in this.ToList()
			        where e.EventType == EventType.Lost
			        where e.Date.Date > studystartDate.Date
			        where e.Date.Date < studyEndDate.Date
			        select e.Hours).Sum();
		}

		public int TotalHours()
		{
			return (from e in this.ToList()
			        where e.EventType != EventType.Lost
			        select e.Hours).Sum();
		}

		public int HoursCompleteForTopic(string topicId)
		{
			return (from e in this.ToList()
			        where e.Complete == true
			        where e.TopicId == topicId
				where (e.EventType == EventType.Study || e.EventType == EventType.Revision)
			        select e.Hours).Sum();
		}

		public int TotalHoursForTopic(string topicId)
		{
			return (from e in this.ToList()
			        where e.TopicId == topicId
				    where (e.EventType == EventType.Study || e.EventType == EventType.Revision)
			        select e.Hours).Sum();
		}

		public int TotalHoursLeftForTopic(string topicId)
		{
			return (from e in this.ToList()
				where e.TopicId == topicId
				where (e.EventType == EventType.Study || e.EventType == EventType.Revision)
				where e.Complete == false
				select e.Hours).Sum();
		}


		public int TotalHoursForTopicInFuture(string topicId)
		{
			return (from e in this
				where e.TopicId == topicId
				where (e.EventType == EventType.Study || e.EventType == EventType.Revision)
				where e.Date.Date > DomainApp.TodaysDate.Date
				select e.Hours).Sum();
		}

		public int HoursCompleteForExamByType(string examId, EventType eventType)
		{
			return (from e in this.ToList()
			        where e.Complete == true
			        where e.ExamId == examId
					where e.EventType == eventType
			        select e.Hours).Sum();
		}
		
		public int TotalHoursForExamByType(string examId, EventType eventType)
		{
			return (from e in this.ToList()
			        where e.ExamId == examId
					where e.EventType == eventType
			        select e.Hours).Sum();
		}

		public int HoursCompleteForExam(string examId)
		{
			return (from e in this.ToList()
				where e.ExamId == examId
				where (e.EventType == EventType.Study || e.EventType == EventType.Revision)
				select e.HoursComplete).Sum();
		}

		public int HoursCompleteForExam(string examId,
                                        DateTime date)
		{
			return (from e in this.ToList()
					where e.ExamId == examId
					where e.Date.Date == date.Date
					where (e.EventType == EventType.Study || e.EventType == EventType.Revision)
					select e.HoursComplete).Sum();
		}

        public int HoursLeftForExam(string examId,
										  DateTime date)
		{
			return (from e in this.ToList()
					where e.ExamId == examId
					where e.Date.Date == date.Date
					where (e.EventType == EventType.Study || e.EventType == EventType.Revision)
					select e.HoursLeft).Sum();
		}

		public int TotalHoursForExam(string examId)
		{
			return (from e in this.ToList()
				where e.ExamId == examId
				where (e.EventType == EventType.Study || e.EventType == EventType.Revision)
				select e.Hours).Sum();
		}

		public int HoursToGo()
		{
			return (from e in this.ToList()
					where e.Complete == false
					where (e.EventType == EventType.Study || e.EventType == EventType.Revision)
					select e.Hours).Sum();
		}


		/// <summary>
		/// Days until last event.
		/// </summary>
		/// <returns>
		/// The until last event.
		/// </returns>
		public int DaysUntilLastEvent()
		{
			DateTime lastDate = GetLastEventDay;
			return Convert.ToInt32(lastDate.Subtract(DomainApp.TodaysDate.AddDays(-1)).TotalDays);
		}


		public List<Event> CompletedEvents()
		{
			return (from e in this.ToList()
			        where e.Complete
			        select e).ToList();
		}

		public List<Event> LostEvents()
		{
			return (from e in this.ToList()
			        where e.EventType == EventType.Lost
			        select e).ToList();
		}

		public List<Event> MissedEvents(DateTime date)
		{
			return (from e in this.ToList()
			        where e.Complete == false
					where (e.Date.Date <= date.Date)
			        select e).ToList();
		}

		public bool DayCompleted(DateTime date)
		{
			var eventsComplete = from e in this.ToList()
								 where e.Complete == true
								 where e.Date.Date == date.Date
								 select e;

			bool complete = false;

			if (eventsComplete != null && eventsComplete.Count() > 0)
			{
				complete = true;
			}

			return complete;
		}

	}
}

