using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;


namespace Retail.BusinessEntity.Data
{
    public interface ICreditClassDataManager : IDataManager
    {
        List<CreditClass> GetCreditClasses();

        bool AreCreditClassUpdated(ref object updateHandle);

        bool Insert(CreditClass CreditClassItem, out int insertedId);

        bool Update(CreditClass CreditClassItem);
    }
}
