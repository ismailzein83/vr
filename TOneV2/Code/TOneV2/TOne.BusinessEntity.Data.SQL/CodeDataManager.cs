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
            DataTable dtSuppliersCodeInfo = CarrierAccountDataManager.BuildCarrierAccountInfoTable(activeSuppliers);
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

        public List<Code> GetCodes(int zoneID, DateTime effectiveOn)
        {
            return GetItemsSP("BEntity.sp_Codes_GetByZone", (reader) =>
            {
                return new Entities.Code
                {
                    ID = (long)reader["ID"],
                    Value = reader["Code"] as string,
                    ZoneId = (int)reader["ZoneID"],
                    BeginEffectiveDate = reader["BeginEffectiveDate"] as Nullable<DateTime>,
                    EndEffectiveDate = GetReaderValue<Nullable<DateTime>>(reader, "EndEffectiveDate"),
                    CodeGroup = reader["CodeGroup"] as string,
                    SupplierId = reader["SupplierID"] as string,
                    Timestamp = (byte[])reader["timestamp"]
                };
            }, zoneID, effectiveOn);
        }
        public SuppliersCodes GetCodesByCodePrefixGroup(string codePrefix, DateTime effectiveOn, bool isFuture, List<Entities.CarrierAccountInfo> activeSuppliers, out List<string> distinctCodes, out HashSet<Int32> supplierZoneIds, out HashSet<Int32> saleZoneIds)
        {
            DataTable dtSuppliersCodeInfo = CarrierAccountDataManager.BuildCarrierAccountInfoTable(activeSuppliers);
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
                            if (String.Compare(code.SupplierId, "sys", true) == 0)
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

        public List<CodeGroupInfo> GetCodeGroupsByCustomer(string customerId)
        {
            return GetItemsSP("[BEntity].[sp_CodeGroup_GetByCustomer]", (reader) =>
            {
                return new Entities.CodeGroupInfo
                {
                    Code = reader["Code"] as string,
                    Name = reader["Name"] as string
                };
            }, customerId);
        }
        public string GetCodeGroupName(int codeGroupId)
        {
            return ExecuteScalarSP("[BEntity].[sp_CodeGroup_GetName]", codeGroupId) as string;
        }

        public List<string> GetDistinctCodePrefixes(int codePrefixLength, DateTime effectiveOn, bool isFuture)
        {
            return GetItemsSP("[BEntity].[sp_Code_GetDistinctCodePrefixes]", (reader) => reader["CodePrefix"] as string, codePrefixLength, effectiveOn, isFuture);
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




        public Dictionary<string, CodeGroupInfo> GetCodeGroupsByCodePrefix(string codePrefix)
        {
            Dictionary<string, CodeGroupInfo> codeGroups = new Dictionary<string, CodeGroupInfo>();
            ExecuteReaderSP("BEntity.SP_CodeGroup_GetByCodePrefix", (reader) =>
           {
               while (reader.Read())
               {
                   CodeGroupInfo codeGroup = new CodeGroupInfo()
                   {
                       Code = reader["Code"] as string,
                       Name = reader["Name"] as string
                   };
                   codeGroups.Add(codeGroup.Code, codeGroup);
               };
           }, codePrefix);
            return codeGroups;
        }

        public List<Code> GetSupplierCodes(string supplierId, char rootCode, DateTime whenEffective)
        {
            return GetItemsSP("BEntity.sp_Code_GetBySupplier", (reader) =>
            {
                return new Entities.Code
                {
                    ID = (long)reader["ID"],
                    Value = reader["Code"] as string,
                    ZoneId = (int)reader["ZoneID"],
                    BeginEffectiveDate = reader["BeginEffectiveDate"] as Nullable<DateTime>,
                    EndEffectiveDate = GetReaderValue<Nullable<DateTime>>(reader, "EndEffectiveDate"),
                    CodeGroup = reader["CodeGroup"] as string,
                    SupplierId = reader["SupplierID"] as string,
                    ServicesFlag = GetReaderValue<Int16>(reader, "ServicesFlag"),
                    Name = reader["Name"] as string,
                };
            }, whenEffective, supplierId, rootCode);
        }
    }
}
