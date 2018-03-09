using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class ANumberSaleCodeDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IANumberSaleCodeDataManager
    {
        #region Fields / Constructors

        public ANumberSaleCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "MainDBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public IEnumerable<ANumberSaleCode> GetFilteredANumberSaleCodes(int aNumberGroupId, List<int> sellingNumberPlanIds)
        {
            string sellingNumberPlanIdsAsString = null;
            if (sellingNumberPlanIds != null && sellingNumberPlanIds.Count() > 0)
                sellingNumberPlanIdsAsString = string.Join<int>(",", sellingNumberPlanIds);
            return GetItemsSP("TOneWhS_BE.sp_ANumberSaleCode_GetFiltered", ANumberSaleCodeMapper, aNumberGroupId, sellingNumberPlanIdsAsString);
        }
       
        #endregion

       
        #region  Mappers

        ANumberSaleCode ANumberSaleCodeMapper(IDataReader reader)
        {
            ANumberSaleCode aNumberSaleCode = new ANumberSaleCode
            {
                ANumberSaleCodeId = GetReaderValue<long>(reader, "ID"),
                ANumberGroupId = GetReaderValue<int>(reader, "ANumberGroupID"),
                SellingNumberPlanId = GetReaderValue<int>(reader, "SellingNumberPlanID"),
                Code = reader["Code"] as string,
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };

            return aNumberSaleCode;
        }

        #endregion
    }
}
