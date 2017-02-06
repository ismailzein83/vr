using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL.SourceDataManger
{
    public class SourceSpecialRequestDataManager : BaseSQLDataManager
    {
        readonly bool _getEffectiveOnly;
        public SourceSpecialRequestDataManager(string connectionString, bool getEffectiveOnly)
            : base(connectionString, false)
        {
            _getEffectiveOnly = getEffectiveOnly;
        }
        public IEnumerable<SourceSpecialRequest> GetSpecialRequestRules()
        {
            return GetItemsText(query_getSpecialRequestRules, SourceSpecialRequestRuleMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@GetEffectiveOnly", _getEffectiveOnly));
            });
        }

        private SourceSpecialRequest SourceSpecialRequestRuleMapper(IDataReader reader)
        {
            int sourceId = (int)reader["SpecialRequestID"];
            return new SourceSpecialRequest
            {
                BED = GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
                EED = GetReaderValue<DateTime?>(reader, "EndEffectiveDate"),
                Code = reader["Code"] as string,
                ExcludedCodes = reader["ExcludedCodes"] as string,
                IncludeSubCode = string.IsNullOrEmpty((reader["IncludeSubCodes"] as string)) ? false : (reader["IncludeSubCodes"] as string).Equals("Y"),
                ExcludedCodesList = GetExcludedCodes(reader["ExcludedCodes"] as string),
                Reason = reader["Reason"] as string,
                CustomerId = reader["CustomerId"] as string,
                SourceId = sourceId.ToString(),
                SupplierOption = new SpecialRequestSupplierOption
                {
                    SupplierId = reader["SupplierId"] as string,
                    NumberOfTries = GetReaderValue<byte>(reader, "NumberOfTries"),
                    Percentage = GetReaderValue<byte>(reader, "Percentage"),
                    Priority = GetReaderValue<byte>(reader, "Priority"),
                    ForcedOption = GetReaderValue<byte>(reader, "SpecialRequestType") == 1,
                    SourceId = sourceId
                }
            };
        }
        HashSet<string> GetExcludedCodes(string codes)
        {
            if (string.IsNullOrEmpty(codes))
                return null;
            return new HashSet<string>(codes.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).OrderBy(s => s));
        }
        /// <summary>
        /// Get Effective Only(Avoid Data Error
        /// </summary>
        const string query_getSpecialRequestRules = @"SELECT       sr.[SpecialRequestID]
                                                                  ,sr.[CustomerID]
                                                                  ,sr.[ZoneID]
                                                                  ,sr.[Code]
                                                                  ,sr.[SupplierID]
                                                                  ,sr.[Priority]
                                                                  ,sr.[NumberOfTries]
                                                                  ,sr.[SpecialRequestType]
                                                                  ,sr.[BeginEffectiveDate]
                                                                  ,sr.[EndEffectiveDate]
                                                                  ,sr.[Percentage]
                                                                  ,sr.[Reason]
                                                                  ,sr.[IncludeSubCodes]
                                                                  ,sr.[ExcludedCodes]
                                                                  ,sr.[IsEffective]
                                                           FROM   [SpecialRequest] sr
                                                           WHERE  ((@GetEffectiveOnly = 0 and sr.BeginEffectiveDate <= getdate()) or (@GetEffectiveOnly = 1 and sr.IsEffective = 'Y'))
                                                           Order By sr.CustomerID, sr.Code, sr.BeginEffectiveDate desc";

    }
}
