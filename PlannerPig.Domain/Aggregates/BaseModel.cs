using System;
using Newtonsoft.Json;

namespace StudyPlanner.Domain.Aggregates
{
    public class BaseModel : BaseNotify, IDirty
    {
        public string Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }


        public DateTimeOffset UpdatedAt { get; set; }


        public string Version { get; set; }


        public bool Deleted { get; set; }

        [JsonIgnore]
        public bool IsDirty
        {
            get;
            set;
        }
    }
}