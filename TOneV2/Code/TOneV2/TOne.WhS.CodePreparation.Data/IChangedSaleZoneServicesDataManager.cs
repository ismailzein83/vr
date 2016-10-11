using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Data;

namespace TOne.WhS.CodePreparation.Data
{
    public interface IChangedSaleZoneServicesDataManager : IDataManager, IBulkApplyDataManager<ChangedZoneServices>
    {
        long ProcessInstanceId { set; }
        void ApplyChangedZonesServicesToDB(object preparedObject);
    }
}
