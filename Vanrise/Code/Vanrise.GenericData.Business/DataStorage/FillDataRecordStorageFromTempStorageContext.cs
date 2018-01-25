using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class FillDataRecordStorageFromTempStorageContext : IFillDataRecordStorageFromTempStorageContext
    {
        public DataStore DataStore { get; set; }

        public DataRecordStorage DataRecordStorage { get; set; }

        public TempStorageInformation TempStorageInformation { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public RecordFilterGroup RecordFilterGroup { get; set; }
    }
}