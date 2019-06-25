using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{ 
    public class ListDataRecordTypeGridViewColumnInfo
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public GridColumnSettings GridColumnSettings { get; set; }
    }
    public class DataRecordTypeGridViewColumnsInput
    {
        public List<ListDataRecordTypeGridViewColumnInfo> ColumnsInfo { get; set; }
    }
}
