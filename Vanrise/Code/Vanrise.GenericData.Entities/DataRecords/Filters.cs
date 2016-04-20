using System;
using System.Collections.Generic;
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

    public enum StringRecordFilterOperator { Equals = 0, NotEquals = 1, StartsWith = 2, NotStartsWith = 3, EndsWith = 4, NotEndsWith = 5, Contains = 6, NotContains = 7}
    public class StringRecordFilter : RecordFilter
    {
        public StringRecordFilterOperator CompareOperator { get; set; }

        public string Value { get; set; }
    }

    public enum NumberRecordFilterOperator { Equals = 0, NotEquals = 1, Greater = 2, GreaterOrEquals = 3, Less = 4, LessOrEquals = 5}
    public class NumberRecordFilter : RecordFilter
    {
        public NumberRecordFilterOperator CompareOperator { get; set; }

        public Decimal Value { get; set; }
    }

    public enum DateTimeRecordFilterOperator { Equals = 0, NotEquals = 1, Greater = 2, GreaterOrEquals = 3, Less = 4, LessOrEquals = 5 }
    public class DateTimeRecordFilter : RecordFilter
    {
        public DateTimeRecordFilterOperator CompareOperator { get; set; }

        public DateTime Value { get; set; }
    }

    public class BooleanRecordFilter : RecordFilter
    {
        public bool IsTrue { get; set; }
    }

    public enum ListRecordFilterOperator { In = 0, NotIn = 1}
    public abstract class ListRecordFilter<T> : RecordFilter
    {
        public ListRecordFilterOperator CompareOperator { get; set; }

        public List<T> Values { get; set; }
    }

    public class NumberListRecordFilter : ListRecordFilter<Decimal>
    {

    }

}
