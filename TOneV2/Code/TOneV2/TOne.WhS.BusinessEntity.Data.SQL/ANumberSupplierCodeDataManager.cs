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
    public class ANumberSupplierCodeDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IANumberSupplierCodeDataManager
    {
        #region Fields / Constructors

        public ANumberSupplierCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "MainDBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public IEnumerable<ANumberSupplierCode> GetFilteredANumberSupplierCodes(int aNumberGroupId, List<int> supplierIds)
        {
            string supplierIdsAsString = null;
            if (supplierIds != null && supplierIds.Count() > 0)
                supplierIdsAsString = string.Join<int>(",", supplierIds);
            return GetItemsSP("TOneWhS_BE.sp_ANumberSupplierCode_GetFiltered", ANumberSupplierCodeMapper, aNumberGroupId, supplierIdsAsString);
        }

        public IEnumerable<ANumberSupplierCode> GetEffectiveAfterBySupplierId(int supplierId,DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_ANumberSupplierCode_GetEffectiveAfterBySupplierId", ANumberSupplierCodeMapper, supplierId, effectiveOn);
        }
       
        public bool Insert(List<ANumberSupplierCodeToClose> listOfSupplierCodesToClose, long aNumberSupplierCodeId, int aNumberGroupID, int supplierID, string code, DateTime effectiveOn)
        {
            DataTable dtSupplierCodesToClose = BuildANumberSupplierCodesTableToClose(listOfSupplierCodesToClose);
            return ExecuteNonQuerySPCmd("[TOneWhS_BE].[sp_ANumberSupplierCode_Insert]", (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@ANumberSupplierCodeId", aNumberSupplierCodeId));
                cmd.Parameters.Add(new SqlParameter("@ANumberGroupID", aNumberGroupID));
                cmd.Parameters.Add(new SqlParameter("@SupplierID", supplierID));
                cmd.Parameters.Add(new SqlParameter("@Code", code));
                cmd.Parameters.Add(new SqlParameter("@BED", effectiveOn));
                var dtPrm = new SqlParameter("@ANumberSupplierCodeClose", SqlDbType.Structured);
                dtPrm.Value = dtSupplierCodesToClose;
                cmd.Parameters.Add(dtPrm);
            }) > 0;
        }
        public ANumberSupplierCode GetANumberSupplierCode(long aNumberSupplierCodeId)
        {
            return GetItemSP("sp_ANumberSupplierCode_Get", ANumberSupplierCodeMapper, aNumberSupplierCodeId);
        }
        public bool Close(long aNumberSupplierCodeId, DateTime effectiveOn)
        {
            return ExecuteNonQuerySP("[TOneWhS_BE].[sp_ANumberSupplierCode_Close]", aNumberSupplierCodeId, effectiveOn) > 0;
        }
        internal static DataTable BuildANumberSupplierCodesTableToClose(List<ANumberSupplierCodeToClose> listOfANumberSupplierCodesToClose)
        {
            DataTable dtANumberSupplierCodeToClose = GetANumberSupplierCodeTableToClose();
            dtANumberSupplierCodeToClose.BeginLoadData();
            foreach (var SupplierCodeToClose in listOfANumberSupplierCodesToClose)
            {
                DataRow drSupplierZoneServiceToClose = dtANumberSupplierCodeToClose.NewRow();
                drSupplierZoneServiceToClose["ANumberSupplierCodeIdToClose"] = SupplierCodeToClose.ANumberSupplierCodeId;
                drSupplierZoneServiceToClose["ANumberSupplierCodeEEDToClose"] = SupplierCodeToClose.CloseDate;
                dtANumberSupplierCodeToClose.Rows.Add(drSupplierZoneServiceToClose);
            }
            dtANumberSupplierCodeToClose.EndLoadData();
            Console.WriteLine(dtANumberSupplierCodeToClose);
            return dtANumberSupplierCodeToClose;
        }

        private static DataTable GetANumberSupplierCodeTableToClose()
        {
            DataTable dtANumberSupplierCodeTableToClose = new DataTable();
            dtANumberSupplierCodeTableToClose.Columns.Add("ANumberSupplierCodeIdToClose", typeof(long));
            dtANumberSupplierCodeTableToClose.Columns.Add("ANumberSupplierCodeEEDToClose", typeof(DateTime));
            return dtANumberSupplierCodeTableToClose;
        }

        #endregion

       
        #region  Mappers

        ANumberSupplierCode ANumberSupplierCodeMapper(IDataReader reader)
        {
            ANumberSupplierCode aNumberSupplierCode = new ANumberSupplierCode
            {
                ANumberSupplierCodeId = GetReaderValue<long>(reader, "ID"),
                ANumberGroupId = GetReaderValue<int>(reader, "ANumberGroupID"),
                SupplierId = GetReaderValue<int>(reader, "SupplierID"),
                Code = reader["Code"] as string,
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };

            return aNumberSupplierCode;
        }

        #endregion
    }
}
