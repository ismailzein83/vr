﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class EmptyRecordFilter : RecordFilter
    {

    }

    public class NonEmptyRecordFilter : RecordFilter
    {

    }

    public enum StringRecordFilterOperator
    {
        [Description(" = ")]
        Equals = 0,
        [Description(" <> ")]
        NotEquals = 1,
        [Description(" Starts With ")]
        StartsWith = 2,
        [Description(" Not Starts With ")]
        NotStartsWith = 3,
        [Description(" Ends With ")]
        EndsWith = 4,
        [Description(" Not Ends With ")]
        NotEndsWith = 5,
        [Description(" Contains ")]
        Contains = 6,
        [Description(" Not Contains ")]
        NotContains = 7
    }
    public class StringRecordFilter : RecordFilter
    {
        public StringRecordFilterOperator CompareOperator { get; set; }

        public string Value { get; set; }
    }

    public enum NumberRecordFilterOperator
    {
        [Description(" = ")]
        Equals = 0,
        [Description(" <> ")]
        NotEquals = 1,
        [Description(" > ")]
        Greater = 2,
        [Description(" >= ")]
        GreaterOrEquals = 3,
        [Description(" < ")]
        Less = 4,
        [Description(" <= ")]
        LessOrEquals = 5
    }
    public class NumberRecordFilter : RecordFilter
    {
        public NumberRecordFilterOperator CompareOperator { get; set; }

        public Decimal Value { get; set; }
    }

    public enum DateTimeRecordFilterOperator
    {
        [Description(" = ")]
        Equals = 0,
        [Description(" <> ")]
        NotEquals = 1,
        [Description(" > ")]
        Greater = 2,
        [Description(" >= ")]
        GreaterOrEquals = 3,
        [Description(" < ")]
        Less = 4,
        [Description(" <= ")]
        LessOrEquals = 5
    }
    public class DateTimeRecordFilter : RecordFilter
    {
        public DateTimeRecordFilterOperator CompareOperator { get; set; }

        public DateTime Value { get; set; }
    }

    public class BooleanRecordFilter : RecordFilter
    {
        public bool IsTrue { get; set; }
    }

    public enum ListRecordFilterOperator
    {
        [Description("IN")]
        In = 0,
        [Description("NOT IN")]
        NotIn = 1
    }
    public abstract class ListRecordFilter<T> : RecordFilter
    {
        public ListRecordFilterOperator CompareOperator { get; set; }

        public List<T> Values { get; set; }
    }

    public class NumberListRecordFilter : ListRecordFilter<Decimal>
    {

    }

}
