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

        public IEnumerable<ANumberSaleCode> GetEffectiveAfterBySellingNumberPlanId(int sellingNumberPlanId,DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_ANumberSaleCode_GetEffectiveAfterBySellingNumberPlanId", ANumberSaleCodeMapper, sellingNumberPlanId, effectiveOn);
        }
      
        public bool Insert(List<ANumberSaleCodeToClose> listOfSaleCodesToClose, long aNumberSaleCodeId, int aNumberGroupID, int sellingNumberPlanID, string code, DateTime effectiveOn)
        {
            DataTable dtSaleCodesToClose = BuildANumberSaleCodesTableToClose(listOfSaleCodesToClose);
            return ExecuteNonQuerySPCmd("[TOneWhS_BE].[sp_ANumberSaleCode_Insert]", (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@ANumberSaleCodeId", aNumberSaleCodeId));
                cmd.Parameters.Add(new SqlParameter("@ANumberGroupID", aNumberGroupID));
                cmd.Parameters.Add(new SqlParameter("@SellingNumberPlanID", sellingNumberPlanID));
                cmd.Parameters.Add(new SqlParameter("@Code", code));
                cmd.Parameters.Add(new SqlParameter("@BED", effectiveOn));
                var dtPrm = new SqlParameter("@ANumberSaleCodeClose", SqlDbType.Structured);
                dtPrm.Value = dtSaleCodesToClose;
                cmd.Parameters.Add(dtPrm);
            }) > 0;
        }

        public ANumberSaleCode GetANumberSaleCode(long aNumberSaleCodeId)
        {
            return GetItemSP("sp_ANumberSaleCode_Get", ANumberSaleCodeMapper, aNumberSaleCodeId);
        }

        public bool Close(long aNumberSaleCodeId, DateTime effectiveOn)
        {
            return ExecuteNonQuerySP("[TOneWhS_BE].[sp_ANumberSaleCode_Close]", aNumberSaleCodeId, effectiveOn) > 0;
        }

        internal static DataTable BuildANumberSaleCodesTableToClose(List<ANumberSaleCodeToClose> listOfANumberSaleCodesToClose)
        {
            DataTable dtANumberSaleCodeToClose = GetANumberSaleCodeTableToClose();
            dtANumberSaleCodeToClose.BeginLoadData();
            foreach (var saleCodeToClose in listOfANumberSaleCodesToClose)
            {
                DataRow drSupplierZoneServiceToClose = dtANumberSaleCodeToClose.NewRow();
                drSupplierZoneServiceToClose["ANumberSaleCodeIdToClose"] = saleCodeToClose.ANumberSaleCodeId;
                drSupplierZoneServiceToClose["ANumberSaleCodeEEDToClose"] = saleCodeToClose.CloseDate;
                dtANumberSaleCodeToClose.Rows.Add(drSupplierZoneServiceToClose);
            }
            dtANumberSaleCodeToClose.EndLoadData();
            Console.WriteLine(dtANumberSaleCodeToClose);
            return dtANumberSaleCodeToClose;
        }

        private static DataTable GetANumberSaleCodeTableToClose()
        {
            DataTable dtANumberSaleCodeTableToClose = new DataTable();
            dtANumberSaleCodeTableToClose.Columns.Add("ANumberSaleCodeIdToClose", typeof(long));
            dtANumberSaleCodeTableToClose.Columns.Add("ANumberSaleCodeEEDToClose", typeof(DateTime));
            return dtANumberSaleCodeTableToClose;
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
