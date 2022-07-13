using Newtonsoft.Json;

namespace StudyPlanner.Domain.Aggregates
{
	public class TodoItem : BaseModel
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

		string text;

		[JsonProperty(PropertyName = "text")]
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				SetPropertyChanged(ref text, value);
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

		[JsonProperty(PropertyName = "hoursComplete")]
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
		public string TextFormatted
		{
			get
			{
				return !string.IsNullOrEmpty(Text) ? string.Format("{0}h {1}", Hours, Text) : "N/A";
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

		string studyTopicId;

		[JsonProperty(PropertyName = "studyTopicId")]
		public string StudyTopicId
		{
			get
			{
				return studyTopicId;
			}
			set
			{
				SetPropertyChanged(ref studyTopicId, value);
			}
		}

		string revisionTopicId;

		[JsonProperty(PropertyName = "revisionTopicId")]
		public string RevisionTopicId
		{
			get
			{
				return revisionTopicId;
			}
			set
			{
				SetPropertyChanged(ref revisionTopicId, value);
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

		string topicName;

		[JsonProperty(PropertyName = "topicName")]
		public string TopicName
		{
			get
			{
				return topicName;
			}
			set
			{
				SetPropertyChanged(ref topicName, value);
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

		public string GetTopicId()
		{
			if (!string.IsNullOrEmpty(studyTopicId))
				return studyTopicId;
			else if (!string.IsNullOrEmpty(revisionTopicId))
				return revisionTopicId;
			else
				return null;
		}
	}
}

