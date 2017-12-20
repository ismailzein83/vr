using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Data
{
    public interface ITelesAccountEnterpriseDataManager:IDataManager
    {
        void SaveAccountEnterprisesDIDs(List<AccountEnterpriseDID> accountEnterprisesDIDs);
    }
}
