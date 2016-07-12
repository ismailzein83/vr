using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Data
{
    public interface IStatusDefinitionDataManager: IDataManager
    {
        List<StatusDefinition> GetStatusDefinition();

        bool AreStatusDefinitionUpdated(ref object updateHandle);
    }
}
