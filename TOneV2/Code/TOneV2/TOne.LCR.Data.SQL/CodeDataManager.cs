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
        public List<string> GetUpdatedCodesSuppliers(byte[] updatedAfter, out byte[] newLastTimestamp)
        {
            List<string> updatedSuppliers = new List<string>();
            byte[] newLastTimestamp_Local = null;
            ExecuteReaderSP("LCR.sp_Code_GetUpdatedSuppliers", (reader) =>
                {
                    if (reader.Read())
                        newLastTimestamp_Local = reader["LastTimestamp"] as byte[];
                    reader.NextResult();
                    while (reader.Read())
                        updatedSuppliers.Add(reader["SupplierID"] as string);
                }, updatedAfter);
            newLastTimestamp = newLastTimestamp_Local;
            return updatedSuppliers;
        }

        public List<String> GetDistinctCodes(bool isFuture)
        {
            return GetItemsSP("LCR.sp_Code_GetDistinctCodes", (reader) =>
            {
                return reader["Code"] as string;
            }, isFuture);
        }

        public List<String> GetDistinctCodes(bool isFuture, char firstDigit)
        {
            return GetItemsSP("LCR.sp_Code_GetDistinctCodesByFirstDigit", (reader) =>
            {
                return reader["Code"] as string;
            }, isFuture, firstDigit);
        }

        public List<String> GetDistinctCodes(bool isFuture, string codeGroup)
        {
            return GetItemsSP("LCR.sp_Code_GetDistinctCodesByCodeGroup", (reader) =>
            {
                return reader["Code"] as string;
            }, isFuture, codeGroup);
        }

        public List<String> GetDistinctCodes(bool isFuture, DateTime effectiveOn, bool getChangedGroupsOnly)
        {
            return GetItemsSP("LCR.sp_Code_GetDistinctCodesForChangedGroups", (reader) =>
            {
                return reader["Code"] as string;
            }, isFuture, effectiveOn, getChangedGroupsOnly);
        }

        public List<LCRCode> GetSupplierCodes(string supplierId, DateTime effectiveOn)
        {
            return GetItemsSP("LCR.sp_Code_GetBySupplier", LCRCodeMapper, supplierId, effectiveOn);
        }

        public void LoadCodesByUpdatedSuppliers(byte[] codeUpdatedAfter, DateTime effectiveOn, Action<string, List<LCRCode>> onSupplierCodesReady)
        {
            ExecuteReaderSP("LCR.sp_Code_GetByUpdatedSuppliers", (reader) =>
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
                }, codeUpdatedAfter, effectiveOn);
        }


        public void LoadCodesForActiveSuppliers(bool isFuture, Action<string, List<LCRCode>> onSupplierCodesReady)
        {
            ExecuteReaderSP("LCR.sp_Code_GetForActiveSuppliers", (reader) =>
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
            }, isFuture);
        }

        public void LoadCodesForActiveSuppliers(bool isFuture, char firstDigit, Action<string, List<LCRCode>> onSupplierCodesReady)
        {
            ExecuteReaderSP("LCR.sp_Code_GetForActiveSuppliersByFirstDigit", (reader) =>
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
            }, isFuture, firstDigit);
        }

        public void LoadCodesForActiveSuppliers(bool isFuture, string codeGroup, Action<string, List<LCRCode>> onSupplierCodesReady)
        {
            ExecuteReaderSP("LCR.sp_Code_GetForActiveSuppliersByCodeGroup", (reader) =>
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
            }, isFuture, codeGroup);
        }

        public void LoadCodesForActiveSuppliers(bool isFuture, DateTime effectiveOn, bool getChangedGroupsOnly, Action<string, List<LCRCode>> onSupplierCodesReady)
        {
            ExecuteReaderSP("LCR.sp_Code_GetForActiveSuppliersAndChangedCodeGroups", (reader) =>
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
            }, isFuture, effectiveOn, getChangedGroupsOnly);
        }

        public Dictionary<string, Dictionary<string, LCRCode>> GetOrderedCodesForActiveSuppliers(bool isFuture)
        {
            Dictionary<string, Dictionary<string, LCRCode>> suppliersOrderedCodes = new Dictionary<string, Dictionary<string, LCRCode>>();
            ExecuteReaderSP("LCR.sp_Code_GetOrderedForActiveSuppliers", (reader) =>
            {
                string currentSupplierId = null;
                Dictionary<string, LCRCode> supplierCodes = null;
                string lastAddedCode = null;
                while (reader.Read())
                {
                    LCRCode code = LCRCodeMapper(reader);
                    if (currentSupplierId != code.SupplierId)
                    {
                        if (supplierCodes != null)
                            suppliersOrderedCodes.Add(currentSupplierId, supplierCodes);

                        currentSupplierId = code.SupplierId;
                        supplierCodes = new Dictionary<string,LCRCode>();
                        lastAddedCode = null;
                    }
                    if (code.Value != lastAddedCode)
                    {
                        supplierCodes.Add(code.Value, code);
                        lastAddedCode = code.Value;
                    }
                }
                if (supplierCodes != null && supplierCodes.Count > 0)
                    suppliersOrderedCodes.Add(currentSupplierId, supplierCodes);
            }, isFuture);
            return suppliersOrderedCodes;
        }

        public Dictionary<string, List<LCRCode>> GetOrderedCodesForActiveSuppliers2(bool isFuture)
        {
            Dictionary<string, List<LCRCode>> allCodes = new Dictionary<string, List<LCRCode>>();
            ExecuteReaderSP("LCR.sp_Code_GetOrderedForActiveSuppliers", (reader) =>
            {
                string currentCode = null;
                List<LCRCode> subCodes = null;
                string lastAddedSupplier = null;
                while (reader.Read())
                {
                    LCRCode code = LCRCodeMapper(reader);
                    if (currentCode != code.Value)
                    {
                        if (subCodes != null)
                            allCodes.Add(currentCode, subCodes);

                        currentCode = code.Value;
                        subCodes = new List<LCRCode>();
                        lastAddedSupplier = null;
                    }
                    if (code.SupplierId != lastAddedSupplier)
                    {
                        subCodes.Add(code);
                        lastAddedSupplier = code.SupplierId;
                    }
                }
                if (subCodes != null && subCodes.Count > 0)
                    allCodes.Add(currentCode, subCodes);
            }, isFuture);
            return allCodes;
        }

        public List<LCRCode> GetAllCodesOrdered()
        {
            string query = @"SELECT c.[ID]
		                          ,c.[Code]
		                          ,Convert(bigint, LEFT(c.[Code] + '00000000000000000000', 18)) as CodeNumber
		                          ,c.[ZoneID]
		                          ,c.[BeginEffectiveDate]
		                          ,c.[EndEffectiveDate]
		                          ,c.[IsEffective]
		                          ,c.[UserID]
		                          ,c.[timestamp]
		                          ,z.CodeGroup 
		                          ,z.SupplierID
                          FROM [Code] c With (nolock)
                          JOIN Zone z with (nolock) on c.ZoneID = z.ZoneID
                          WHERE c.IsEffective = 'Y' 
                          ORDER by CodeNumber desc, z.SupplierID";
            return GetItemsText(query, LCRCodeMapper, null);
        }

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
    }
}
