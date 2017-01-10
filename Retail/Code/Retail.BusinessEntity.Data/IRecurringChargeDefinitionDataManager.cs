using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IRecurringChargeDefinitionDataManager : IDataManager
    {
        List<RecurringChargeDefinition> GetRecurringChargeDefinitions();

        bool AreRecurringChargeDefinitionUpdated(ref object updateHandle);

        bool Insert(RecurringChargeDefinition recurringChargeDefinition);

        bool Update(RecurringChargeDefinition recurringChargeDefinition);
    }
}
