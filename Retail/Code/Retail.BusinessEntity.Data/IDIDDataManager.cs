using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Data;


namespace Retail.BusinessEntity.Data
{
    public interface IDIDDataManager : IDataManager
    {
        List<DID> GetAllDIDs();
        bool Insert(DID dID, out int dIDId);
        bool Update(DID dID);
        bool AreDIDsUpdated(ref object updateHandle);
    }
}
