using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TABS;
using System.Data.SqlClient;
using System.Data;
using TOne.Caching;

namespace TOne.Business
{
    public class ProtCodeMap
    {
        TOneCacheManager _cacheManager;

        public ProtCodeMap(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Find the best matching Code for the given supplier in the Code Map.
        /// </summary>
        /// <param name="code">The destination code (or dialed number)</param>
        /// <param name="supplier">The supplier</param>
        /// <returns>A Code object or null if not found</returns>
        public TABS.Code Find(string code, TABS.CarrierAccount supplier, DateTime whenEffective)
        {
            if (string.IsNullOrEmpty(code)) return null;
            TABS.Code found = null;
            Dictionary<string, List<Code>> supplierCodes = GetSupplierCodes(supplier, code[0], whenEffective);
            if (supplierCodes != null)
            {
                List<Code> matchingCodes = null;
                StringBuilder codeValue = new StringBuilder(GetDigits(code));
                while (found == null && codeValue.Length > 0)
                {

                    if (supplierCodes.TryGetValue(codeValue.ToString(), out matchingCodes))
                    {
                        foreach (TABS.Code possibleCode in matchingCodes)
                        {
                            if (possibleCode.IsEffectiveOn(whenEffective))
                            {
                                found = possibleCode;
                                bool IsCodeGroup = TABS.CodeGroup.All.Keys.Contains(codeValue.ToString());
                                if (found.Zone.IsHaveMatchingCodeGroup == false)
                                    found.Zone.IsHaveMatchingCodeGroup = IsCodeGroup;
                                found.Zone.IsCodeGroup = IsCodeGroup;
                                break;
                            }
                        }
                    }
                    codeValue.Length--;
                }
            }
            return found;
        }

        Dictionary<string, List<Code>> GetSupplierCodes(CarrierAccount supplier, char rootCode, DateTime whenEffective)
        {
            return _cacheManager.GetOrCreateObject(String.Format("GetSupplierCodes_{0}_{1}_{2: ddMMMyy}", supplier.CarrierAccountID, rootCode, whenEffective),
                TOne.Entities.CacheObjectType.SupplierCodes,
                () =>
                {
                    List<TABS.Code> allCodes = new List<TABS.Code>();
                   
                    Dictionary<int, TABS.Zone> zones = new Dictionary<int, TABS.Zone>();


                    Dictionary<string, List<Code>> supplierCodes = new Dictionary<string, List<Code>>();

                    using (var sqlConnection = new System.Data.SqlClient.SqlConnection(DataConfiguration.Default.Properties["connection.connection_string"].ToString()))//hibernate.connection.connection_string
                    {
                        sqlConnection.Open();
                        string sqlQuery = string.Format(@"
                    SELECT 
                        C.ID, 
                        C.Code, 
                        C.BeginEffectiveDate, 
                        C.EndEffectiveDate,
                        C.ZoneID, 
                        Z.CodeGroup, 
                        Z.Name,
                        Z.SupplierID,
                        Z.ServicesFlag,
                        Z.BeginEffectiveDate as ZoneBED,
                        Z.EndEffectiveDate as ZoneEED
                    FROM Code C WITH(NOLOCK), Zone Z WITH(NOLOCK),CarrierAccount CA
                    WHERE C.ZoneID = Z.ZoneID 
                        AND Z.SupplierID = CA.CarrierAccountID AND CA.ActivationStatus = 2
                        AND (C.EndEffectiveDate IS NULL OR (C.EndEffectiveDate > '{0:yyyy-MM-dd}' And C.BeginEffectiveDate<>C.EndEffectiveDate)) 
                        AND (Z.EndEffectiveDate IS NULL OR (Z.EndEffectiveDate > '{0:yyyy-MM-dd}' And Z.BeginEffectiveDate<>Z.EndEffectiveDate)) 
                        AND SupplierID = '{1}'
                        AND c.Code like '{2}%'           
                    ORDER BY Z.SupplierID, C.Code, C.BeginEffectiveDate DESC",
                                whenEffective, supplier.CarrierAccountID, rootCode);

                        SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
                        command.CommandTimeout = 0;
                        var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                        int resultcount = 0;
                        while (reader.Read())
                        {
                            resultcount++;
                            int index = -1;
                            var code = new TABS.Code()
                            {
                                ID = reader.GetInt64(++index),
                                Value = reader.GetString(++index),
                                BeginEffectiveDate = reader.IsDBNull(++index) ? null : (DateTime?)reader.GetDateTime(index),
                                EndEffectiveDate = reader.IsDBNull(++index) ? null : (DateTime?)reader.GetDateTime(index)
                            };

                            int zoneID = reader.GetInt32(++index);

                            TABS.Zone zone = null;
                            if (!zones.TryGetValue(zoneID, out zone))
                            {
                                zone = new TABS.Zone()
                                {
                                    ZoneID = zoneID,
                                    CodeGroup = reader.IsDBNull(++index) ? TABS.CodeGroup.None : TABS.CodeGroup.All[reader.GetString(index)],
                                    Name = reader.GetString(++index),
                                    Supplier = supplier,

                                };
                                ++index;
                                zone.ServicesFlag = reader.GetInt16(++index);
                                zone.BeginEffectiveDate = reader.IsDBNull(++index) ? null : (DateTime?)reader.GetDateTime(index);
                                zone.EndEffectiveDate = reader.IsDBNull(++index) ? null : (DateTime?)reader.GetDateTime(index);
                                zones.Add(zoneID, zone);
                            }

                            code.Zone = zone;
                            allCodes.Add(code);
                        }
                    }

                    foreach (TABS.Code code in allCodes)
                    {
                        string codeValue = GetDigits(code.Value);
                        List<Code> codeList = null;
                        if (!supplierCodes.TryGetValue(codeValue, out codeList))
                        {
                            codeList = new List<Code>();
                            supplierCodes[codeValue] = codeList;
                        }
                        codeList.Add(code);
                    }

                    return supplierCodes;
                });
           
        }

        protected string GetDigits(string codeValue)
        {
            StringBuilder sb = new StringBuilder(codeValue.Length);
            for (int i = 0; i < codeValue.Length; i++)
            {
                char c = codeValue[i];
                if (char.IsDigit(c)) sb.Append(c);
            }
            return sb.ToString();
        }

    }
}
