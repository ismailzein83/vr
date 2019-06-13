using System;
using System.Collections.Generic;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordFieldInfoFilter
    {
        public List<string> IncludedFieldNames { get; set; }
        public IEnumerable<IDataRecordFieldFilter> Filters { get; set; }

        public bool ExcludeFormula { get; set; }
      
    }
    public interface IDataRecordFieldFilter
    {
        bool IsExcluded(IDataRecordFieldFilterContext context);

    }


    public interface IDataRecordFieldFilterContext
    {
        DataRecordField DataRecordField { get; }

    }
    public class DataRecordFieldFilterContext: IDataRecordFieldFilterContext
    {
        public DataRecordField DataRecordField { get; set; }
    }
}