using System;
using System.Collections.Generic;
using System.Data;
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

        public PriceList GetPriceListById(int priceListId)
        {
            return GetItemSP("[BEntity].[sp_PriceList_GetByID]", PriceListMapper, priceListId);
        }

        PriceList PriceListMapper(IDataReader reader)
        {
            return new PriceList()
            {
                PriceListID = (int)reader["PriceListID"],
                Description = reader["Description"] as string,
                BeginEffectiveDate = reader["BeginEffectiveDate"] as Nullable<DateTime>,
                EndEffectiveDate = reader["EndEffectiveDate"] as Nullable<DateTime>,
                CurrencyId = reader["CurrencyId"] as string
            };
        }
        

        public bool SavePriceList(PriceList pricelist, out int priceListId)
        {
            object id;

            int recordesEffected = ExecuteNonQuerySP("BEntity.[sp_PriceList_Insert]", out id, 
                                                                                      pricelist.SupplierId, 
                                                                                      pricelist.CustomerId,
                                                                                      pricelist.Description, 
                                                                                      pricelist.CurrencyId, 
                                                                                      pricelist.BeginEffectiveDate, 
                                                                                      pricelist.EndEffectiveDate,pricelist.SourceFileName,
                                                                                      pricelist.UserId, 
                                                                                      pricelist.IsSend );

            priceListId = (recordesEffected > 0) ? (Int32)id : -1;
            return (recordesEffected > 0);
        }
    }
}
