using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordVanriseType : Vanrise.Entities.IVanriseType
    {
        string _uniqueName;
        public DataRecordVanriseType(string dataRecordTypeName)
        {
            if (String.IsNullOrEmpty(dataRecordTypeName))
                throw new ArgumentNullException("dataRecordTypeName");

            _uniqueName = string.Format("GenericData_DataRecordType_{0}", dataRecordTypeName);
        }

        public string UniqueTypeName
        {
            get { return _uniqueName; }
        }
    }
}
