using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class CodeDataManager : BaseTOneDataManager, ICodeDataManager
    {
        public Dictionary<string, Dictionary<string, Code>> GetCodes(char firstDigit, DateTime effectiveOn, bool isFuture, List<Entities.CarrierAccountInfo> activeSuppliers, out List<string> distinctCodes)
        {
            DataTable dtSuppliersCodeInfo = CarrierDataManager.BuildCarrierAccountInfoTable(activeSuppliers);
            Dictionary<string, Dictionary<string, Code>> allSuppliersCodes = new Dictionary<string, Dictionary<string, Code>>();
            List<string> distinctCodes_Internal = new List<string>();
            distinctCodes = distinctCodes_Internal;
            ExecuteReaderSPCmd("BEntity.sp_Code_GetByFirstDigitForActiveSuppliers",
                    (reader) =>
                    {
                        string currentCode = null;
                        while (reader.Read())
                        {
                            Code code = CodeMapper(reader);
                            Dictionary<string, Code> supplierCodes;
                            if (!allSuppliersCodes.TryGetValue(code.SupplierId, out supplierCodes))
                            {
                                supplierCodes = new Dictionary<string, Code>();
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
                        var dtPrm = new SqlParameter("@ActiveSuppliersInfo", SqlDbType.Structured);
                        dtPrm.Value = dtSuppliersCodeInfo;
                        cmd.Parameters.Add(dtPrm);

                        cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                        cmd.Parameters.Add(new SqlParameter("@IsFuture", isFuture));
                    });
            return allSuppliersCodes;
        }

    
        public SuppliersCodes GetCodesByCodePrefixGroup(string codePrefix, DateTime effectiveOn, bool isFuture, List<Entities.CarrierAccountInfo> activeSuppliers, out List<string> distinctCodes, out HashSet<Int32> supplierZoneIds, out HashSet<Int32> saleZoneIds)
        {
            DataTable dtSuppliersCodeInfo = CarrierDataManager.BuildCarrierAccountInfoTable(activeSuppliers);
            //Dictionary<string, Dictionary<string, Code>> allSuppliersCodes = new Dictionary<string, Dictionary<string, Code>>();
            SuppliersCodes allSuppliersCodes = new SuppliersCodes();
            allSuppliersCodes.Codes = new Dictionary<string, SupplierCodes>();

         

            HashSet<int> saleZoneIds_Internal = new HashSet<int>();
            HashSet<int> supplierZoneIds_Internal = new HashSet<int>();

            saleZoneIds = saleZoneIds_Internal;
            supplierZoneIds = supplierZoneIds_Internal;

            List<string> distinctCodes_Internal = new List<string>();
            distinctCodes = distinctCodes_Internal;
            ExecuteReaderSPCmd("BEntity.sp_Code_GetByCodePrefixGroupForActiveSuppliers",
                    (reader) =>
                    {
                        string currentCode = null;
                        while (reader.Read())
                        {
                            Code code = CodeMapper(reader);
                            SupplierCodes supplierCodes;
                            if (code.SupplierId.ToLower() == "sys")
                                saleZoneIds_Internal.Add(code.ZoneId);
                            else
                                supplierZoneIds_Internal.Add(code.ZoneId);
                            if (!allSuppliersCodes.Codes.TryGetValue(code.SupplierId, out supplierCodes))
                            {
                                supplierCodes = new SupplierCodes();
                                supplierCodes.Codes = new Dictionary<string, Code>();
                                allSuppliersCodes.Codes.Add(code.SupplierId, supplierCodes);
                            }
                            if (!supplierCodes.Codes.ContainsKey(code.Value))
                                supplierCodes.Codes.Add(code.Value, code);

                            if (currentCode != code.Value)
                            {
                                currentCode = code.Value;
                                distinctCodes_Internal.Add(currentCode);
                            }
                        }
                    },
                    (cmd) =>
                    {
                        cmd.Parameters.Add(new SqlParameter("@CodePrefixGroup", codePrefix));
                        var dtPrm = new SqlParameter("@ActiveSuppliersInfo", SqlDbType.Structured);
                        dtPrm.Value = dtSuppliersCodeInfo;
                        cmd.Parameters.Add(dtPrm);

                        cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                        cmd.Parameters.Add(new SqlParameter("@IsFuture", isFuture));
                    });
            return allSuppliersCodes;
        }


        public List<CodeGroupInfo> GetCodeGroups()
        {
            return GetItemsSP("BEntity.SP_CodeGroup_GetCodeGroups", (reader) =>
            {
                return new Entities.CodeGroupInfo
                {
                    Code = reader["Code"] as string,
                    Name = reader["Name"] as string
                };
            });
        }

        #region Private Methods

        Code CodeMapper(IDataReader reader)
        {
            Code code = new Code
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
