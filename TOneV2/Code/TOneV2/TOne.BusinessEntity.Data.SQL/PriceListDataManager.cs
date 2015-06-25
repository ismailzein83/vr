using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class PriceListDataManager : BaseTOneDataManager, IPriceListDataManager
    {
        public List<PriceList> GetPriceList()
        {
            return GetItemsSP("[BEntity].sp_PriceList_GetByCarrierAccountID", (reader) =>
                {
                    return new PriceList
                    {
                        PriceListID = (int)reader["PriceListID"],
                        Description = reader["[Description]"] as string,
                        BeginEffectiveDate = reader["BeginEffectiveDate"] as Nullable<DateTime>,
                        UserName = reader["Name"] as string
                    };
                }
                );
        }
    }
}
