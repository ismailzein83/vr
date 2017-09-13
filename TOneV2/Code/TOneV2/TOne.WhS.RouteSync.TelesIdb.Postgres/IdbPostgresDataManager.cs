using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Idb;
using Vanrise.Data.Postgres;
using System.Linq;
//using Vanrise.Common;
//using Vanrise.Entities;

namespace TOne.WhS.RouteSync.MVTSRadius.SQL
{
    public class IdbPostgresDataManager : BasePostgresDataManager, IIdbDataManager
    {
        public Guid ConfigId { get { return new Guid("34F3483D-2572-4349-A6ED-3504B2D9E714"); } }

        public IdbPostgresDataManager()
        {

        }

        IdbConnectionString _connectionString;
        List<IdbConnectionString> _redundantConnectionStrings;

        public IdbConnectionString ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public List<IdbConnectionString> RedundantConnectionStrings
        {
            get { return _redundantConnectionStrings; }
            set { _redundantConnectionStrings = value; }
        }

        public void PrepareTables(ISwitchRouteSynchronizerInitializeContext context)
        {
            throw new NotImplementedException();
        }

        public object PrepareDataForApply(List<ConvertedRoute> radiusRoutes)
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
    }

    public class TelesIdbPostgresDataManager : BasePostgresDataManager
    {
        public string ConnectionString { get { return GetConnectionString(); } }

        IdbConnectionString _idbConnectionString;
        public TelesIdbPostgresDataManager(IdbConnectionString idbConnectionString)
        {
            _idbConnectionString = idbConnectionString;
        }
        protected override string GetConnectionString()
        {
            return _idbConnectionString.ConnectionString;
        }

        #region Constants

        #endregion
    }
}