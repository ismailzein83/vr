using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Cataleya.Data;
using TOne.WhS.RouteSync.Cataleya.Entities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.Cataleya.Postgres
{
    public class SyncDataManager : BasePostgresDataManager, ISyncDataManager
    {
        public Guid ConfigId { get { return new Guid("CAD5F46B-F182-462A-AACC-B862ACD11AEB"); } }
        public string SwitchId { get; set; }
        public string _connectionString { get; set; }

        public SyncDataManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public object InitialiazeStreamForDBApply()
        {
            throw new NotImplementedException();
        }

        public void WriteRecordToStream(ConvertedRoute record, object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public void PrepareTables(ISwitchRouteSynchronizerInitializeContext context)
        {
            //SwitchSyncOutput switchSyncOutput;
            //ExecIdbPostgresDataManagerAction((telesIdbPostgresDataManager, dataManagerIndex) =>
            //{
            //    telesIdbPostgresDataManager.PrepareTables();
            //}, context.SwitchName, context.SwitchId, null, context.WriteBusinessHandledException, true, "initializing", out switchSyncOutput);
            //context.SwitchSyncOutput = switchSyncOutput;
        }

        public object PrepareDataForApply(List<ConvertedRoute> routes)
        {
            throw new NotImplementedException();
        }

        public void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            throw new NotImplementedException();
        }

        public void SwapTables(ISwapTableContext context)
        {
            throw new NotImplementedException();
        }

        public void PrepareTables(IRouteInitializeContext context)
        {
            throw new NotImplementedException();
        }

        public List<CarrierAccountVersionInfo> GetCarrierAccountsPreviousVersionNumbers(IGetCarrierAccountsPreviousVersionNumbersContext context)
        {
            throw new NotImplementedException();
        }
    }
}