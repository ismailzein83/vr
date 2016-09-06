using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Data;


namespace Retail.BusinessEntity.Data
{
    public interface IPopDataManager : IDataManager
    {
        List<Pop> GetAllPops();
        bool Insert(Pop pop,out int popId);
        bool Update(Pop pop);
        bool ArePopsUpdated(ref object updateHandle);
    }
}
