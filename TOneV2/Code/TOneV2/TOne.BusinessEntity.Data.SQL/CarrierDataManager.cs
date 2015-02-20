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

        public List<CarrierInfo> GetCarriers(CarrierType carrierType)
        {
            return GetItemsSP("BEntity.sp_Carriers_GetCarriers", (reader) =>
            {
                return new CarrierInfo
                {
                    CarrierAccountID = reader["CarrierAccountID"] as string,
                    Name = string.Format("{0}{1}", reader["Name"] as string, reader["NameSuffix"] != DBNull.Value && !string.IsNullOrEmpty(reader["NameSuffix"].ToString()) ? " (" + reader["NameSuffix"] as string + ")" : string.Empty)
                };
            }, carrierType.ToString());
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
