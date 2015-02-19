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
    public class CarrierDataManager : BaseTOneDataManager, ICarrierDataManager
    {
        public List<CarrierAccountInfo> GetActiveSuppliersInfo()
        {
            return GetItemsSP("BEntity.sp_CarrierAccount_GetActiveSuppliersInfo", CarrierAccountInfoMapper);
        }

        #region Private Methods

        private CarrierAccountInfo CarrierAccountInfoMapper(IDataReader reader)
        {
            return new CarrierAccountInfo
            {
                CarrierAccountId = reader["CarrierAccountID"] as string
            };
        }

        internal static DataTable BuildCarrierAccountInfoTable(List<CarrierAccountInfo> carrierAccountsInfo)
        {
            DataTable dtSuppliersCodeInfo = new DataTable();
            dtSuppliersCodeInfo.Columns.Add("CarrierAccountID", typeof(string));
            dtSuppliersCodeInfo.BeginLoadData();
            foreach (var c in carrierAccountsInfo)
            {
                DataRow dr = dtSuppliersCodeInfo.NewRow();
                dr["CarrierAccountID"] = c.CarrierAccountId;
                dtSuppliersCodeInfo.Rows.Add(dr);
            }
            dtSuppliersCodeInfo.EndLoadData();
            return dtSuppliersCodeInfo;
        }

        #endregion
    }
}
