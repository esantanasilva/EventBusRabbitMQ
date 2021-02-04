using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Models
{
    public abstract class Event
    {
        public Event()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            CreationDateEpoch = (long)Math.Floor((DateTime.UtcNow.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
        }

        public Event(Guid id, DateTime createDate)
        {
            Id = id;
            CreationDate = createDate;
            CreationDateEpoch = (long)Math.Floor((createDate.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
        }

        public string ServiceId { get; set; }

        public DateTime CreationDate { get; }
        public Guid Id { get; set; }

        public readonly long CreationDateEpoch;


    }
}
