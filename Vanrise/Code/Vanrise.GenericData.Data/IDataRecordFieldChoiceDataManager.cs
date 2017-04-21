using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IDataRecordFieldChoiceDataManager:IDataManager
    {
        bool AreDataRecordFieldChoicesUpdated(ref object updateHandle);
        bool UpdateDataRecordFieldChoice(DataRecordFieldChoice dataRecordFieldChoice);
        bool AddDataRecordFieldChoice(DataRecordFieldChoice dataRecordFieldChoice);
        IEnumerable<DataRecordFieldChoice> GetDataRecordFieldChoices();
    }
}
