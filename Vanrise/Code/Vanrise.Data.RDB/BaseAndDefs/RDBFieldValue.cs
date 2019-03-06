﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public abstract class RDBFieldValue
    {
        public abstract string StringValue { get; }

        public abstract string StringValueWithEmptyHandling { get; }

        public abstract int IntValue { get; }
        
        public abstract int? NullableIntValue { get; }

        public abstract long LongValue { get; }
        
        public abstract long? NullableLongValue { get; }

        public abstract decimal DecimalValue { get; }
        
        public abstract decimal? NullableDecimalValue { get; }

        public abstract double DoubleValue { get; }
        
        public abstract double? NullableDoubleValue { get; }

        public abstract DateTime DateTimeValue { get; }
        
        public abstract DateTime? NullableDateTimeValue { get; }

        public abstract Guid GuidValue { get; }
        
        public abstract Guid? NullableGuidValue { get; }

        public abstract Boolean BooleanValue { get; }
        
        public abstract Boolean? NullableBooleanValue { get; }

        public abstract byte[] BytesValue { get; }
    }

    public class CommonRDBFieldValue : RDBFieldValue
    {
        Object _value;
        public CommonRDBFieldValue(Object value)
        {
            _value = value;
        }

        static CommonRDBFieldValue()
        {
            s_nullableInlineTypes.Add(typeof(int?), typeof(int));
            s_nullableInlineTypes.Add(typeof(long?), typeof(long));
            s_nullableInlineTypes.Add(typeof(short?), typeof(short));
            s_nullableInlineTypes.Add(typeof(decimal?), typeof(decimal));
            s_nullableInlineTypes.Add(typeof(float?), typeof(float));
            s_nullableInlineTypes.Add(typeof(double?), typeof(double));
        }

        static Dictionary<Type, Type> s_nullableInlineTypes = new Dictionary<Type, Type>();
        internal static Type GetInlineType(Type  nullableType)
        {
            Type inlineType;
            if (s_nullableInlineTypes.TryGetValue(nullableType, out inlineType))
                return inlineType;
            else
                return nullableType;
        }

        protected virtual T GetFieldValue<T>()
        {
            if (_value == null || _value == DBNull.Value)
            {
                return default(T);
            }
            else
            {
                if (_value is T)
                    return (T)_value;
                else
                    return (T)Convert.ChangeType(_value, GetInlineType(typeof(T)));
            }
        }
        
        public override string StringValue
        {
            get { return _value as string; }
        }

        public override string StringValueWithEmptyHandling
        {
            get { return this.StringValue; }
        }

        public override int IntValue
        {
            get { return GetFieldValue<int>(); }
        }
        
        public override int? NullableIntValue
        {
            get { return GetFieldValue<int?>(); }
        }

        public override long LongValue
        {
            get { return GetFieldValue<long>(); }
        }
        
        public override long? NullableLongValue
        {
            get { return GetFieldValue<long?>(); }
        }

        public override decimal DecimalValue
        {
            get { return GetFieldValue<decimal>(); }
        }
        
        public override decimal? NullableDecimalValue
        {
            get { return GetFieldValue<decimal?>(); }
        }

        public override double DoubleValue
        {
            get { return GetFieldValue<double>(); }
        }
        
        public override double? NullableDoubleValue
        {
            get { return GetFieldValue<double?>(); }
        }

        public override DateTime DateTimeValue
        {
            get { return GetFieldValue<DateTime>(); }
        }
        
        public override DateTime? NullableDateTimeValue
        {
            get { return GetFieldValue<DateTime?>(); }
        }

        public override Guid GuidValue
        {
            get { return GetFieldValue<Guid>(); }
        }
        
        public override Guid? NullableGuidValue
        {
            get { return GetFieldValue<Guid?>(); }
        }

        public override bool BooleanValue
        {
            get { return GetFieldValue<bool>(); }
        }
        
        public override bool? NullableBooleanValue
        {
            get { return GetFieldValue<bool?>(); }
        }

        public override byte[] BytesValue
        {
            get { return GetFieldValue<byte[]>(); }
        }
    }

}
