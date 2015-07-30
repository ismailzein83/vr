using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data
{
    public interface IBulkApplyDataManager<T>
    {
        object InitialiazeStreamForDBApply();

        void WriteRecordToStream(T record, object dbApplyStream);

        object FinishDBApplyStream(object dbApplyStream);
    }
}
