using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public enum DataRetrievalResultType { Normal = 0, Excel = 1}
    public class DataRetrievalInput<T> : DataRetrievalInput
    {       
        public T Query { get; set; }
    }

    public class DataRetrievalInput
    {
        public string ResultKey { get; set; }

        public DataRetrievalResultType DataRetrievalResultType { get; set; }

        public int? FromRow { get; set; }

        public int? ToRow { get; set; }

        public string SortByColumnName { get; set; }

        public bool IsSortDescending { get; set; }

        public bool GetSummary { get; set; }
    }
}
