using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class DataRecordFieldTypeTryResolveDifferencesContext : IDataRecordFieldTypeTryResolveDifferencesContext
    {
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public object Changes { get; set; }
    }
}
