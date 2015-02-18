using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.LCR.Entities;
using System.Data;
using System.Data.SqlClient;
using TOne.Data.SQL;

namespace TOne.LCR.Data.SQL
{
    public class CodeDataManager : BaseTOneDataManager, ICodeDataManager
    {
        #region Mappers

        LCRCode LCRCodeMapper(IDataReader reader)
        {
            LCRCode code = new LCRCode
            {
                ID = (long)reader["ID"],
                ZoneId = (int)reader["ZoneID"],
                Value = reader["Code"] as string,
                BeginEffectiveDate = reader["BeginEffectiveDate"] as Nullable<DateTime>,
                EndEffectiveDate = GetReaderValue<Nullable<DateTime>>(reader, "EndEffectiveDate"),
                CodeGroup = reader["CodeGroup"] as string,
                SupplierId = reader["SupplierID"] as string,
                Timestamp = (byte[])reader["timestamp"]
            };
            return code;
        }

        #endregion

        #region Private Methods

        internal static DataTable BuildDistinctCodesWithPossibleValuesTable(TOne.Entities.CodeList distinctCodes)
        {
            DataTable dtDistinctCodesWithPossibleMatches = new DataTable();
            dtDistinctCodesWithPossibleMatches.Columns.Add("DistinctCode", typeof(string));
            dtDistinctCodesWithPossibleMatches.Columns.Add("PossibleMatch", typeof(string));
            dtDistinctCodesWithPossibleMatches.BeginLoadData();
            foreach (var distinctCodeEntry in distinctCodes.CodesWithPossibleMatches)
            {
                foreach (var matchCode in distinctCodeEntry.Value)
                {
                    DataRow dr = dtDistinctCodesWithPossibleMatches.NewRow();
                    dr["DistinctCode"] = distinctCodeEntry.Key;
                    dr["PossibleMatch"] = matchCode;
                    dtDistinctCodesWithPossibleMatches.Rows.Add(dr);
                }
            }
            dtDistinctCodesWithPossibleMatches.EndLoadData();
            return dtDistinctCodesWithPossibleMatches;
        }

        internal static DataTable BuildDistinctCodesTable(TOne.Entities.CodeList distinctCodes)
        {
            DataTable dtDistinctCodes = new DataTable();
            dtDistinctCodes.Columns.Add("DistinctCode", typeof(string));
            dtDistinctCodes.BeginLoadData();
            foreach (var distinctCode in distinctCodes.DistinctCodes)
            {
                DataRow dr = dtDistinctCodes.NewRow();
                dr["DistinctCode"] = distinctCode;
                dtDistinctCodes.Rows.Add(dr);
            }
            dtDistinctCodes.EndLoadData();
            return dtDistinctCodes;
        }

        internal static DataTable BuildSuppliersCodeInfoTable(List<SupplierCodeInfo> suppliersCodeInfo)
        {
            DataTable dtSuppliersCodeInfo = new DataTable();
            dtSuppliersCodeInfo.Columns.Add("SupplierID", typeof(string));
            dtSuppliersCodeInfo.Columns.Add("HasUpdatedCodes", typeof(Boolean));
            dtSuppliersCodeInfo.BeginLoadData();
            foreach (var supplierInfo in suppliersCodeInfo)
            {
                DataRow dr = dtSuppliersCodeInfo.NewRow();
                dr["SupplierID"] = supplierInfo.SupplierId;
                dr["HasUpdatedCodes"] = supplierInfo.HasCodesUpdated;
                dtSuppliersCodeInfo.Rows.Add(dr);
            }
            dtSuppliersCodeInfo.EndLoadData();
            return dtSuppliersCodeInfo;
        }

        #endregion

        public void LoadCodesForActiveSuppliers(DateTime effectiveOn, List<SupplierCodeInfo> suppliersCodeInfo, bool onlySuppliersWithUpdatedCodes, Action<string, List<LCRCode>> onSupplierCodesReady)
        {
            DataTable dtSuppliersCodeInfo = BuildSuppliersCodeInfoTable(suppliersCodeInfo);

            ExecuteReaderSPCmd("LCR.sp_Code_GetForActiveSuppliers",
                (reader) =>
                {
                    string currentSupplierId = null;
                    List<LCRCode> supplierCodes = null;
                    while (reader.Read())
                    {
                        LCRCode code = LCRCodeMapper(reader);
                        if (currentSupplierId != code.SupplierId)
                        {
                            if (supplierCodes != null)
                                onSupplierCodesReady(currentSupplierId, supplierCodes);
                            currentSupplierId = code.SupplierId;
                            supplierCodes = new List<LCRCode>();
                        }
                        supplierCodes.Add(code);
                    }
                    if (supplierCodes != null && supplierCodes.Count > 0)
                        onSupplierCodesReady(currentSupplierId, supplierCodes);
                },
                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@ActiveSuppliersCodeInfo", SqlDbType.Structured);
                    dtPrm.Value = dtSuppliersCodeInfo;
                    dtPrm.TypeName = "LCR.SuppliersCodeInfoType";
                    cmd.Parameters.Add(dtPrm);

                    cmd.Parameters.Add(new SqlParameter("@EffectiveOn", effectiveOn));
                    cmd.Parameters.Add(new SqlParameter("@OnlySuppliersWithUpdateCodes", onlySuppliersWithUpdatedCodes));
                });
        }

        public List<LCRCode> GetAllCodesForActiveSuppliers(DateTime effectiveOn, List<SupplierCodeInfo> suppliersCodeInfo)
        {
            DataTable dtSuppliersCodeInfo = BuildSuppliersCodeInfoTable(suppliersCodeInfo);

            return GetItemsSPCmd("LCR.sp_Code_GetALLForActiveSuppliers",
                LCRCodeMapper,
                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@ActiveSuppliersCodeInfo", SqlDbType.Structured);
                    dtPrm.Value = dtSuppliersCodeInfo;
                    dtPrm.TypeName = "LCR.SuppliersCodeInfoType";
                    cmd.Parameters.Add(dtPrm);

                    cmd.Parameters.Add(new SqlParameter("@EffectiveOn", effectiveOn));
                });
        }

        public Dictionary<string, Dictionary<string, LCRCode>> GetCodesForActiveSuppliers(Char firstDigit, DateTime effectiveOn, List<SupplierCodeInfo> suppliersCodeInfo, out List<string> distinctCodes)
        {
            DataTable dtSuppliersCodeInfo = BuildSuppliersCodeInfoTable(suppliersCodeInfo);
            Dictionary<string, Dictionary<string, LCRCode>> allSuppliersCodes = new Dictionary<string, Dictionary<string, LCRCode>>();
            List<string> distinctCodes_Internal = new List<string>();
            distinctCodes = distinctCodes_Internal;
            ExecuteReaderSPCmd("LCR.sp_Code_GetByFirstDigitForActiveSuppliers",
                    (reader) =>
                    {
                        string currentCode = null;
                        while(reader.Read())
                        {
                            LCRCode code = LCRCodeMapper(reader);
                            Dictionary<string, LCRCode> supplierCodes;
                            if(!allSuppliersCodes.TryGetValue(code.SupplierId, out supplierCodes))
                            {
                                supplierCodes = new Dictionary<string, LCRCode>();
                                allSuppliersCodes.Add(code.SupplierId, supplierCodes);
                            }
                            if (!supplierCodes.ContainsKey(code.Value))
                                supplierCodes.Add(code.Value, code);

                            if (currentCode != code.Value)
                            {
                                currentCode = code.Value;
                                distinctCodes_Internal.Add(currentCode);
                            }
                        }
                    },
                    (cmd) =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@FirstDigit", firstDigit));
                        var dtPrm = new SqlParameter("@ActiveSuppliersCodeInfo", SqlDbType.Structured);
                        dtPrm.Value = dtSuppliersCodeInfo;
                        cmd.Parameters.Add(dtPrm);

                        cmd.Parameters.Add(new SqlParameter("@EffectiveOn", effectiveOn));
                    });
            return allSuppliersCodes;
        }

        public List<SupplierCodeInfo> GetActiveSupplierCodeInfo(DateTime effectiveAfter, DateTime effectiveOn)
        {
            return GetItemsSP("LCR.sp_Code_GetActiveSuppliersInfo", (reader) =>
                {
                    return new SupplierCodeInfo
                    {
                        SupplierId = reader["CarrierAccountID"] as string,
                        HasCodesUpdated = Convert.ToBoolean(reader["HasCodeUpdated"])
                    };
                }, ToDBNullIfDefault(effectiveAfter), effectiveOn);
        }

        public List<string> GetDistinctCodes(List<SupplierCodeInfo> suppliersCodeInfo, DateTime effectiveOn)
        {
            DataTable dtSuppliersCodeInfo = BuildSuppliersCodeInfoTable(suppliersCodeInfo);
            return GetItemsSPCmd("LCR.sp_Code_GetDistinctCodes",
                (reader) => reader["Code"] as string,
                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@ActiveSuppliersCodeInfo", SqlDbType.Structured);
                    dtPrm.Value = dtSuppliersCodeInfo;
                    cmd.Parameters.Add(dtPrm);

                    cmd.Parameters.Add(new SqlParameter("@EffectiveOn", effectiveOn));
                });
        }

        public void LoadCodeMatchesFromDistinctCodes(TOne.Entities.CodeList distinctCodes, DateTime effectiveOn, List<SupplierCodeInfo> suppliersCodeInfo, Action<CodeMatch> onCodeMatchReady)
        {
            DataTable dtDistinctCodesWithPossibleMatches = BuildDistinctCodesWithPossibleValuesTable(distinctCodes);

            DataTable dtSuppliersCodeInfo = BuildSuppliersCodeInfoTable(suppliersCodeInfo);

            ExecuteReaderSPCmd("LCR.sp_Code_GetCodeMatchesByDistinctCodes",
                (reader) =>
                {
                    while (reader.Read())
                        onCodeMatchReady(CodeMatchDataManager.CodeMatchMapper(reader));
                },
                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@DistinctCodesWithPossibleMatches", SqlDbType.Structured);
                    dtPrm.Value = dtDistinctCodesWithPossibleMatches;
                    cmd.Parameters.Add(dtPrm);

                    dtPrm = new SqlParameter("@ActiveSuppliersCodeInfo", SqlDbType.Structured);
                    dtPrm.Value = dtSuppliersCodeInfo;
                    cmd.Parameters.Add(dtPrm);

                    cmd.Parameters.Add(new SqlParameter("@EffectiveOn", effectiveOn));
                });
        }

        /// <summary>
        /// Create DistinctCodesTable, Insert Distinct Codes from CodeMatchCurrent, then Create Primary Key Index
        /// </summary>
        public int CreateandInsertDistinctCodesTable()
        {
            int maximumID;
            object output;
            ExecuteNonQuerySP("LCR.sp_Code_CreateAndInsertDistinctCodesTable", out output, null);
            return int.TryParse(output.ToString(), out maximumID) ? maximumID : 0;
        }


    }
}
