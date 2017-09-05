using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public class ConvertResultToExcelDataContext<T> : IConvertResultToExcelDataContext<T>
    {
        public BigResult<T> BigResult
        {
            get;
            set;
        }

        public ExportExcelSheet MainSheet
        {
            get;
            set;
        }


        public DataRetrievalInput Input
        {
            get;
            set;
        }
    }
}
