using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public interface IGenericSummaryTransformer
    {
        DateTime GetRawItemTime(dynamic rawItem);

        string GetItemKeyFromSummaryItem(dynamic summaryItem);

        void SetSummaryItemFieldsToDataRecord(GenericSummaryItem summaryItem);

        void SetDataRecordFieldsToSummaryItem(GenericSummaryItem summaryItem);
    }
}
