using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Jazz.Entities;

namespace TOne.WhS.Jazz.Data
{
   public interface IFinalTransactionDataManager : IDataManager
    {
        void Insert(DateTime fromDate,DateTime toDate,long processInstanceId);
    }
}
