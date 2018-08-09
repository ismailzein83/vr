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

        public abstract double DoubleValue { get; }

        public abstract double DoubleWithNullHandlingValue { get; }

        public abstract double? NullableDoubleValue { get; }

        public abstract DateTime DateTimeValue { get; }

        public abstract DateTime DateTimeWithNullHandlingValue { get; }

        public abstract DateTime? NullableDateTimeValue { get; }

        public abstract Guid GuidValue { get; }

        public abstract Guid GuidWithNullHandlingValue { get; }

        public abstract Guid? NullableGuidValue { get; }

        public abstract Boolean BooleanValue { get; }

        public abstract Boolean BooleanWithNullHandlingValue { get; }

        public abstract Boolean? NullableBooleanValue { get; }

        public abstract byte[] BytesValue { get; }

        public abstract byte[] NullableBytes { get; }
    }

    public class CommonRDBFieldValue : RDBFieldValue
    {
        Object _value;
        public CommonRDBFieldValue(Object value)
        {
            _value = value;
        }

        protected virtual T GetFieldValue<T>()
        {
            if (_value == null)
                throw new NullReferenceException("_value");
            if (_value == DBNull.Value)
                throw new Exception("_value is DBNull");
            if (_value is T)
                return (T)_value;
            else
                return (T)Convert.ChangeType(_value, typeof(T));
        }

        protected virtual T GetFieldValueWithNullHandling<T>()
        {
            if (_value == null || _value == DBNull.Value)
                return default(T);
            else
                if (_value is T)
                    return (T)_value;
                else
                    return (T)Convert.ChangeType(_value, typeof(T));
        }

        public override string StringValue
        {
            get { return _value as string; }
        }

        public override int IntValue
        {
            get { return GetFieldValue<int>(); }
        }

        public override int IntWithNullHandlingValue
        {
            get { return GetFieldValueWithNullHandling<int>(); }
        }

        public override int? NullableIntValue
        {
            get { return GetFieldValueWithNullHandling<int?>(); }
        }

        public override long LongValue
        {
            get { return GetFieldValue<long>(); }
        }

        public override long LongWithNullHandlingValue
        {
            get { return GetFieldValueWithNullHandling<long>(); }
        }

        public override long? NullableLongValue
        {
            get { return GetFieldValueWithNullHandling<long?>(); }
        }

        public override decimal DecimalValue
        {
            get { return GetFieldValue<decimal>(); }
        }

        public override decimal DecimalWithNullHandlingValue
        {
            get { return GetFieldValueWithNullHandling<decimal>(); }
        }

        public override decimal? NullableDecimalValue
        {
            get { return GetFieldValueWithNullHandling<decimal?>(); }
        }

        public override double DoubleValue
        {
            get { return GetFieldValue<double>(); }
        }

        public override double DoubleWithNullHandlingValue
        {
            get { return GetFieldValueWithNullHandling<double>(); }
        }

        public override double? NullableDoubleValue
        {
            get { return GetFieldValueWithNullHandling<double?>(); }
        }

        public override DateTime DateTimeValue
        {
            get { return GetFieldValue<DateTime>(); }
        }

        public override DateTime DateTimeWithNullHandlingValue
        {
            get { return GetFieldValueWithNullHandling<DateTime>(); }
        }

        public override DateTime? NullableDateTimeValue
        {
            get { return GetFieldValueWithNullHandling<DateTime?>(); }
        }

        public override Guid GuidValue
        {
            get { return GetFieldValue<Guid>(); }
        }

        public override Guid GuidWithNullHandlingValue
        {
            get { return GetFieldValueWithNullHandling<Guid>(); }
        }

        public override Guid? NullableGuidValue
        {
            get { return GetFieldValueWithNullHandling<Guid?>(); }
        }

        public override bool BooleanValue
        {
            get { return GetFieldValue<bool>(); }
        }

        public override bool BooleanWithNullHandlingValue
        {
            get { return GetFieldValueWithNullHandling<bool>(); }
        }

        public override bool? NullableBooleanValue
        {
            get { return GetFieldValueWithNullHandling<bool?>(); }
        }

        public override byte[] BytesValue
        {
            get { return GetFieldValue<byte[]>(); }
        }

        public override byte[] NullableBytes
        {
            get { return GetFieldValueWithNullHandling<byte[]>(); }
        }
    }

}
