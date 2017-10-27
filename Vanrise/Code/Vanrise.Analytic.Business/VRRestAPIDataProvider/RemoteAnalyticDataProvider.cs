using System;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Analytic.Business
{
    public class RemoteAnalyticDataProvider : IRemoteAnalyticDataProvider
    {
        Guid _analyticTableId;

        public RemoteAnalyticDataProvider(Guid analyticTableId)
        {
            _analyticTableId = analyticTableId;
        }

        public Vanrise.Entities.IDataRetrievalResult<AnalyticRecord> GetFilteredRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            AnalyticTable analyticTable = new AnalyticTableManager().GetAnalyticTableById(_analyticTableId);
            analyticTable.ThrowIfNull("analyticTable", _analyticTableId);
            analyticTable.Settings.ThrowIfNull("analyticTable.Settings", _analyticTableId);
            analyticTable.Settings.DataProvider.ThrowIfNull("analyticTable.Settings.DataProvider", _analyticTableId);

            VRRestAPIAnalyticDataProvider vrRestAPIAnalyticDataProvider = analyticTable.Settings.DataProvider.CastWithValidate<VRRestAPIAnalyticDataProvider>("analyticTable.Settings.DataProvider");

            input.Query.TableId = vrRestAPIAnalyticDataProvider.RemoteAnalyticTableId;

            var vrRestAPIAnalyticQueryInterceptor =
                vrRestAPIAnalyticDataProvider.VRRestAPIAnalyticQueryInterceptor.CastWithValidate<VRRestAPIAnalyticQueryInterceptor>("vrRestAPIAnalyticDataProvider.VRRestAPIAnalyticQueryInterceptor");

            vrRestAPIAnalyticQueryInterceptor.PrepareQuery(new VRRestAPIAnalyticQueryInterceptorContext() { VRConnectionId = vrRestAPIAnalyticDataProvider.VRConnectionId, Query = input.Query });

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(vrRestAPIAnalyticDataProvider.VRConnectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            var clonedInput = Vanrise.Common.Utilities.CloneObject<DataRetrievalInput<AnalyticQuery>>(input);
            clonedInput.IsAPICall = true;

            if (input.DataRetrievalResultType == DataRetrievalResultType.Excel)
                return connectionSettings.Post<DataRetrievalInput<AnalyticQuery>, RemoteExcelResult<AnalyticRecord>>("/api/VR_Analytic/Analytic/GetFilteredRecords", clonedInput, true);
            else
                return connectionSettings.Post<DataRetrievalInput<AnalyticQuery>, AnalyticSummaryBigResult<AnalyticRecord>>("/api/VR_Analytic/Analytic/GetFilteredRecords", clonedInput, true);
        }
    }
}
