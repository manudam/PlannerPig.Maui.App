using System;
using System.Collections.Generic;
using System.Linq;
using StudyPlanner.Domain.Aggregates;

namespace StudyPlanner.Domain.Services
{
	public class CalculationService
	{
		private ExamList exams;
		private TopicList topics;
		private EventList currentEvents;
		private List<UnallocatedTopic> unallocatedTopics;

		private static CalculationService instance;

		public static CalculationService Instance
		{
			get 
			{
				if (instance == null) 
				{
					instance = new CalculationService ();
				}

				return instance;
			}
		}

		public List<UnallocatedTopic> UnallocatedTopics
		{
			get
			{
				return unallocatedTopics;
			}

			set
			{
				unallocatedTopics = value;
			}
		}

        /// <summary>
        /// Calculates the topic hours.
        /// </summary>
        /// <returns>The topic hours.</returns>
        /// <param name="topics">Topics.</param>
        /// <param name="totalHoursAvailable">Total hours available.</param>
        public TopicList CalculateTopicHours(TopicList topics,
                                             int totalHoursAvailable)
        {
                
            var totalHours = topics.TotalHours();
            var HoursPlannedSoFar = 0;

            foreach (var topic in topics)
            {
                var topicWeight = topic.TopicWeight(totalHours);
                var topicInitHrPlanned = topic.HoursPlanned;
                
                if (topicInitHrPlanned>0)
                {
                    topic.IsComplete = false;
                }
                else
                {
                    topic.IsComplete = true;
                 }
                topic.HoursPlanned = Convert.ToInt32(Math.Round(totalHoursAvailable * topicWeight, MidpointRounding.AwayFromZero));
                HoursPlannedSoFar = HoursPlannedSoFar + topic.HoursPlanned;
                topic.Stickinatend = false; //is this the right place to set the default?
                
                if (topicInitHrPlanned>0 && topic.HoursPlanned==0) //minimum 1h as long as there was something before recalc, so a topic doesnt disappear due to reducing total hrs
                {
                    topic.HoursPlanned = 1;
                    if (HoursPlannedSoFar > totalHoursAvailable)
                    {
                        topic.Stickinatend = true;
                    }
                }
            }

            //exclude stickinatend 
            var totalNewHoursPlanned = topics
                .Where(p=> p.Stickinatend==false)
                .Sum(p => p.HoursPlanned);

            //If the hours don't match then assign or take off hours based on rank. 
            if (totalNewHoursPlanned != totalHoursAvailable)
            {
                var rankedTopics = topics
                    .OrderByDescending(p => p.HoursPlanned)
                    .ToList();

                //check if full of 1h's in which case we need to reorder by sortorder to stick the last ones at the end
                if (totalNewHoursPlanned>totalHoursAvailable)
                {
                    var countof = rankedTopics
                        .Where(p => p.HoursPlanned > 1)
                        .Count();
                    if (countof<= totalNewHoursPlanned-totalHoursAvailable)
                    {
                        rankedTopics = rankedTopics
                            .OrderByDescending(p => p.SortOrder)
                            .ToList();
                    }
                }

                int index = 0;
                while (totalNewHoursPlanned != totalHoursAvailable)
                {
					var topic = rankedTopics[index];
					if (totalNewHoursPlanned > totalHoursAvailable)
					{
                        topic.HoursPlanned--;
						totalNewHoursPlanned--;
					}
					else
					{
                        topic.HoursPlanned++;
						totalNewHoursPlanned++;
					}

                    if (index < rankedTopics.Count() - 1)
                    {
                        index++;
                    }
                    else
                    {
                        index = 0;
                    }
                }
            }

            //don't let topic hour get to 0 if not yet complete and make it stickinatend
            foreach (var calcdtopic in topics)
            {
                if (calcdtopic.HoursPlanned==0 && calcdtopic.IsComplete==false)
                {
                    calcdtopic.HoursPlanned = 1;
                    calcdtopic.Stickinatend = true;
                }
            }

            return topics;
        }

		public EventList AllocateEvents(ExamList exams,
		                                TopicList topicsInput,
                                        EventList otherevents=null, int startadjust=0, int todayhrs=0)
		{
            //take the examlist and topic list and sort them
            //var orderedExams = new ExamList();
            //orderedExams.AddRange(exams.OrderBy(p => p.ExamDate));
			var orderedTopics = new TopicList();
            orderedTopics.AddRange(topicsInput.OrderBy(p => p.SortOrder));

            var totaleventList_study = new EventList();
            var totaleventList_studyandrev = new EventList();
            var totaleventList_all = new EventList();


            if (exams.Count > 0)
			{
				foreach (var exam in exams)
				{
                    //var currentEventList = new EventList();
                    //var eventsForExam = eventsToBeRescheduled
                    //                   .Where(p => p.ExamId == exam.Id)
                    //	.ToList();

                    //foreach (var eventForExam in eventsForExam)
                    //{
                    //	currentEventList.Add(eventForExam);
                    //}
                    var orderedExamTopics = new TopicList();
                    orderedExamTopics.AddRange(orderedTopics.Where(p => p.ExamId == exam.Id));

					totaleventList_study = AddStudySection(exam, orderedExamTopics, startadjust, todayhrs);
					totaleventList_studyandrev = AddExamSection(totaleventList_study, exam);

                    foreach (var ievent in totaleventList_studyandrev)
                    {
                        totaleventList_all.Add(ievent);
                    }
                }
			}

            foreach (var otherevent in otherevents)
            {
                totaleventList_all.Add(otherevent);
            }

            var orderedEvents = totaleventList_all
				.OrderBy (p => p.Date)
				.OrderBy (p => p.EventType)
				.ToList();

            EventList orderedEventList = new EventList();
            foreach (var orderedEvent in orderedEvents)
			{
				orderedEventList.Add(orderedEvent);
			}

			return orderedEventList;
		}

        /// <summary>
        /// Adds the study section.
        /// </summary>
        /// <returns>The study section.</returns>
        /// <param name="eventList">Event list.</param>
        /// <param name="exam">Exam.</param>
        /// <param name="currentEventList">Current event list.</param>
		private EventList AddStudySection(Exam exam, TopicList examtopics, int startadjust, int overwritetoday=0)
		{
            DateTime studyDate = exam.ActualStudyStartDate().AddDays(startadjust);
            DateTime studyEndDate = exam.ActualStudyEndDate();

            var generalRevisionStartDate = exam.ExamDate.AddDays(-1 * exam.RegroupDays);

			var studyEvents = new EventList();

			int t = -1;
			int topicAssign = 0;
			bool newTopic = true;
			int dayHours = 0;
			bool newDay = true;
			Topic topic = null;

			TopicList allTopics = new TopicList();
            TopicList TopicsforEnd = new TopicList();

			foreach (var sortedTopic in examtopics)
			{
                if (sortedTopic.Stickinatend==false) //allocate the stickinatend at the end
                {
                    allTopics.Add(sortedTopic);
                }
                else
                {
                    TopicsforEnd.Add(sortedTopic);
                }
			}

			while (studyDate < generalRevisionStartDate.AddDays(1))
			{
				if (newTopic)
				{
					t++;

					if (t >= allTopics.Count)
					{
						break;
					}

					topic = allTopics[t];

                    var assignedHours = topic.HoursPlanned + topic.HoursAssignedToTasks;

					if (assignedHours > 0 && assignedHours < .5)
					{
						UnallocatedTopics.Add(
							new UnallocatedTopic
							{
								TopicName = topic.Name,
								ExamName = exam.Name,
								EventType = EventType.Study,
							}
						);
					}

                    topicAssign = assignedHours;
				}

				if (newDay)
				{
                    if  (studyDate== exam.ActualStudyStartDate().AddDays(startadjust) && overwritetoday>0)  //only on first day check if dayhour should be overwritten with the passed parameter
                    {
                        studyDate = studyDate.AddDays(1);
                        dayHours = overwritetoday;
                    }
                    else
                    {
                        studyDate = studyDate.AddDays(1);

                        var timetableException = exam
                        .TimetableExceptions
                        .Where(p => (p.ExamId == topic.ExamId || string.IsNullOrEmpty(p.ExamId)))
                        .FirstOrDefault(d => d.Date.Date == studyDate.Date);

                        if (timetableException == null)
                        {
                            var timetable = exam
                                .Timetables
                                .FirstOrDefault(d => d.Day == studyDate.DayOfWeek);

                            dayHours = timetable != null ? timetable.Hours : 0;
                        }
                        else
                        {
                            dayHours = timetableException.Hours;
                        }
                    }
				}

				//if (!currentEventList.DayCompleted(studyDate) &&
				//	currentEventList.EventTypeByDate(studyDate) != EventType.General_Revision &&
				//	currentEventList.EventTypeByDate(studyDate) != EventType.Exam &&
				//	currentEventList.EventTypeByDate(studyDate) != EventType.Lost)
				//{
					if (topicAssign > dayHours)
					{
						if (dayHours != 0)
						{
							studyEvents.Add(new Event
							{
								ExamId = topic.ExamId,
								TopicId = topic.Id,
								Title = topic.Name,
                                ExamName = exam.Name,
                                EventType = EventType.Study,
								Date = studyDate,
								Hours = dayHours,
								Complete = false,
							});
						}
						topicAssign = (topicAssign - dayHours);
						newTopic = false;
						dayHours = 0;
						newDay = true;
					}
					else if (topicAssign == dayHours && topicAssign != 0)
					{
						studyEvents.Add(new Event
						{
							ExamId = topic.ExamId,
							TopicId = topic.Id,
							Title = topic.Name,
							ExamName = exam.Name,
							EventType = EventType.Study,
							Date = studyDate,
							Hours = topicAssign,
							Complete = false,
						});

						topicAssign = 0;
						newTopic = true;
						dayHours = 0;
						newDay = true;
					}
					else
					{
						if (topicAssign != 0)
						{
							studyEvents.Add(new Event
							{
								ExamId = topic.ExamId,
								TopicId = topic.Id,
								Title = topic.Name,
                                ExamName = exam.Name,
                                EventType = EventType.Study,
								Date = studyDate,
								Hours = topicAssign,
								Complete = false,
							});
							dayHours = dayHours - topicAssign;
							topicAssign = 0;

						}
						newTopic = true;
						newDay = false;
					}
				//}
				//else
				//{
				//	newTopic = false;
				//	newDay = true;
				//}
			}

            foreach (var topicleft in TopicsforEnd)
            {
                studyEvents.Add(new Event
                {
                    ExamId = topicleft.ExamId,
                    TopicId = topicleft.Id,
                    Title = topicleft.Name,
                    ExamName = exam.Name,
                    EventType = EventType.Study,
                    Date = studyDate, //last calculated study date
                    Hours = topicleft.HoursPlanned,
                    Complete = false,
                });
            }

            var eventList = new EventList();

			foreach (var studyEvent in studyEvents)
			{
				eventList.Add(studyEvent);
			}

			return eventList;
		}

        /// <summary>
        /// Adds the exam section.
        /// </summary>
        /// <returns>The exam section.</returns>
        /// <param name="eventList">Event list.</param>
        /// <param name="exam">Exam.</param>
        /// <param name="currentEventList">Current event list.</param>
		private EventList AddExamSection(EventList eventList, Exam exam)
		{
			var lastEvent = eventList.GetLastEventDayByType(EventType.Study);
			DateTime eventDate = lastEvent != null ? lastEvent.Date.AddDays(1) : exam.ExamDate.AddDays(-1 * exam.RegroupDays);;

			//Create the exam
			eventList.Add(new Event
			{
				ExamId = exam.Id,
				TopicId = null,
				Title = exam.Name,
				ExamName = exam.Name,
				EventType = EventType.Exam,
				Date = exam.ExamDate,
				Hours = 0,
				Complete = false,
			});

			var examDate = exam.ExamDate;
			var freeDate = examDate;
			freeDate = new DateTime(freeDate.Year, freeDate.Month, freeDate.Day, 0, 0, 0);

			while (eventDate < freeDate)
			{
				if (exam.ExamDate!=eventDate)
				{
					//if (!currentEventList.DayCompleted(eventDate))
					//{
						eventList.Add(new Event
						{
							ExamId = exam.Id,
							TopicId = null,
							Title = string.Empty,
							ExamName = exam.Name,
							EventType = EventType.General_Revision,
							Date = eventDate,
							Hours = 0,
							Complete = false,
						});
					//}
				}

				eventDate = eventDate.AddDays(1);
			}

			return eventList;
   		}



       
	}
}

