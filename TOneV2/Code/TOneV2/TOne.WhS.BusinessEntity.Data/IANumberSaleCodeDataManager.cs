using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IANumberSaleCodeDataManager : IDataManager
    {
        IEnumerable<ANumberSaleCode> GetFilteredANumberSaleCodes(int aNumberGroupID, List<int> sellingNumberPlanIds);
        IEnumerable<ANumberSaleCode> GetEffectiveAfterBySellingNumberPlanId(int sellingNumberPlanId, DateTime effectiveOn);
        bool Insert(List<ANumberSaleCodeToClose> listOfSaleCodesToClose, long aNumberSaleCodeId, int aNumberGroupID, int sellingNumberPlanID, string code, DateTime effectiveOn);
        ANumberSaleCode GetANumberSaleCode(long aNumberSaleCodeId);
        bool Close(long aNumberSaleCodeId, DateTime effectiveOn);
    }
}
