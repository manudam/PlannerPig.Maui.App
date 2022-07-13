using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace StudyPlanner.Domain.Aggregates
{
	
	public class Topic : BaseModel
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

        int hoursPlanned;

		[JsonProperty(PropertyName = "HoursPlanned")]
		public int HoursPlanned
		{
			get
			{
				return hoursPlanned;
			}
			set
			{
				SetPropertyChanged(ref hoursPlanned, value);
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
		public bool HasChanged { get; set; }

		[JsonIgnore]
		public List<TodoItem> Tasks { get; set; }

		[JsonIgnore]
		public int HoursAssignedToTasks
		{
			get
			{
				return this.Tasks != null ?
				  Tasks
				  .Where(p => !p.Complete)
						   .Sum(p => p.Hours) : 0;
			}
		}

		int sortOrder;

		[JsonProperty(PropertyName = "sortOrder")]
		public int SortOrder
		{
			get
			{
				return sortOrder;
			}
			set
			{
				SetPropertyChanged(ref sortOrder, value);
			}
		}

		bool unallocated;

		[JsonProperty(PropertyName = "unallocated")]
        public bool Unallocated
        {
            get
            {
                return unallocated;
            }
            set
            {
                SetPropertyChanged(ref unallocated, value);
            }
        }

        bool stickinatend;

        [JsonProperty(PropertyName = "stickinatend")]
        public bool Stickinatend
        {
            get
            {
                return stickinatend;
            }
            set
            {
                SetPropertyChanged(ref stickinatend, value);
            }
        }

        bool isComplete;

        [JsonProperty(PropertyName = "isComplete")]
        public bool IsComplete
        {
            get
            {
                return isComplete;
            }
            set
            {
                SetPropertyChanged(ref isComplete, value);
            }
        }

        DateTime examDate;

		[JsonIgnore]
		public DateTime ExamDate
		{
			get
			{
				return examDate;
			}
			set
			{
				examDate = value;
			}
		}

		[JsonIgnore]
		public string ChartColor
		{
			get
			{
				if (eventType == EventType.Study)
				{
					return "9EC5A1";
				}
				else
				{
					return "E8BB75";
				}
			}

		}

        bool isOpen;

        [JsonIgnore]
        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                isOpen = value;
            }
        }


		public Topic()
		{
			Tasks = new List<TodoItem>();
		}

		public double TopicWeight(double totalHours)
		{
			if (totalHours == 0)
			{
				totalHours = 1;
			}

            return Convert.ToDouble(hoursPlanned) / totalHours;
		}

		public string LongName(Exam exam)
		{
			var examName = exam != null ? exam.Name : string.Empty;

			return (!string.IsNullOrEmpty(examName) ? examName + " " : string.Empty) + this.name;
		}
	}
	
	public class TopicList : List<Topic>
	{
	
		public int TotalHours()
		{
            return this
                .Sum(p => p.HoursPlanned);
		}

		public List<Topic> GetTopicListByType(EventType eventType)
		{
			return this
				.Where (p => p.EventType == eventType)
				.ToList ();
		}

		public int GetTopicListCountByType(EventType eventType)
		{
			return this
				.Where (p => p.EventType == eventType)
				.Count ();
		}
		
	}
}

