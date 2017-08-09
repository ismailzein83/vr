using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.ExcelConversion.Entities
{
    public class ConvertedExcelList
    {
        public string ListName { get; set; }

        public List<ConvertedExcelRecord> Records { get; set; }

        public List<ConvertedExcelRecord> FilteredRecords { get; set; }
    }

    public class ConvertedExcelListsByName : Dictionary<string, ConvertedExcelList>
    {
        public void Add(ConvertedExcelList list)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            this.Add(list.ListName, list);
        }
    }
}
