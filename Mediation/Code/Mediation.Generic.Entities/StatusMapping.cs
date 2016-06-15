using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Mediation.Generic.Entities
{
    public enum EventStatus { Start, Update, Stop }
    public class StatusMapping
    {
        public EventStatus Status { get; set; }
        public object FilterExpression { get; set; }
        public RecordFilterGroup FilterGroup { get; set; }
    }
}
