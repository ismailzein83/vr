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
        public DatabaseConnection DatabaseConnection { get; set; }

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
            //Create CustomerIdentification
            //Create Carrier Account
            //Drop if exist and fill CI temp and fill
            //Drop if exist and fill CarrierAccount temp and fill
            //Create all Routing tables

        }

        public List<CarrierAccountVersionInfo> GetCarrierAccountsPreviousVersionNumbers(IGetCarrierAccountsPreviousVersionNumbersContext context)
        {
            throw new NotImplementedException();
        }
    }
}