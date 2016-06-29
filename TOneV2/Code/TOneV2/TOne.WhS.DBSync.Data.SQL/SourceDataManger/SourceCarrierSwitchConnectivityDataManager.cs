using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCarrierSwitchConnectivityDataManager : BaseSQLDataManager
    {
        public SourceCarrierSwitchConnectivityDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceCarrierSwitchConnectivity> GetSourceCarrierSwitchesConnectivity()
        {
            return GetItemsText(query_getSourceCarrierSwitchesConnectivity, SourceCarrierSwitchConnectiviyMapper, null);
        }

        private SourceCarrierSwitchConnectivity SourceCarrierSwitchConnectiviyMapper(IDataReader arg)
        {
            return new SourceCarrierSwitchConnectivity()
            {
                SourceId = arg["ID"].ToString(),
                SwitchId = GetReaderValue<byte>(arg, "SwitchID"),
                CarrierAccountId = arg["CarrierAccountID"] as string,
                Name = arg["Name"] as string,
                ConnectionType = (SwitchConnectivityType)GetReaderValue<byte>(arg, "ConnectionType"),
                NumberOfChannelsIn = GetReaderValue<int>(arg, "NumberOfChannels_In"),
                NumberOfChannelsOut =GetReaderValue<int>(arg ,"NumberOfChannels_Out"),
                BED = (DateTime)arg["BeginEffectiveDate"],
                EED = GetReaderValue<DateTime?>(arg, "EndEffectiveDate"),
                MarginTotal =GetReaderValue<Single>(arg, "Margin_Total"),
                NumberOfChannelsShared = GetReaderValue<int>(arg, "NumberOfChannels_Shared"),
                Details = arg["Details"] as string,
            };
        }

        const string query_getSourceCarrierSwitchesConnectivity = @"SELECT    [ID], [CarrierAccountID], [SwitchID], [Name], [ConnectionType], [NumberOfChannels_In],
                                                                        [NumberOfChannels_Out], [BeginEffectiveDate], [EndEffectiveDate], [Margin_Total],
                                                                        [NumberOfChannels_Shared], [Details]
                                                                        FROM [dbo].[CarrierSwitchConnectivity] WITH (NOLOCK)";
    }
}
