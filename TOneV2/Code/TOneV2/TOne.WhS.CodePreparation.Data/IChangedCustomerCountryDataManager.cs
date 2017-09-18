using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Data
{
    public interface IChangedCustomerCountryDataManager : IDataManager, Vanrise.Data.IBulkApplyDataManager<ChangedCustomerCountry>
    {
        long ProcessInstanceId { set; }

        void ApplyChangedCustomerCountriesToDB(object preparedObject);
    }
}
