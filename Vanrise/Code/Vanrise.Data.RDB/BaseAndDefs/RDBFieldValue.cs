using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public abstract class RDBFieldValue
    {
        public abstract string StringValue { get; }

        public abstract int IntValue { get; }

        public abstract int IntWithNullHandlingValue { get; }

        public abstract int? NullableIntValue { get; }

        public abstract long LongValue { get; }

        public abstract long LongWithNullHandlingValue { get; }

        public abstract long? NullableLongValue { get; }

        public abstract decimal DecimalValue { get; }

        public abstract decimal DecimalWithNullHandlingValue { get; }

        public abstract decimal? NullableDecimalValue { get; }

        public abstract DateTime DateTimeValue { get; }

        public abstract DateTime DateTimeWithNullHandlingValue { get; }

        public abstract DateTime? NullableDateTimeValue { get; }

        public abstract Guid GuidValue { get; }

        public abstract Guid GuidWithNullHandlingValue { get; }

        public abstract Guid? NullableGuidValue { get; }

        public abstract Boolean BooleanValue { get; }

        public abstract Boolean BooleanWithNullHandlingValue { get; }

        public abstract Boolean? NullableBooleanValue { get; }
    }
}
