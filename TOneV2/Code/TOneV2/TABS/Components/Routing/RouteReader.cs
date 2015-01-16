using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace TABS.Components.Routing
{
    public class RouteReader : IEnumerable<Route>, IEnumerator<Route>, IDisposable
    {
        int _BatchSize = 25000;
        int _totalRoutesRead = 0;
        string _ValidCustomersIdList, _ValidSuppliersIdList;
        RouteSynchType _synchType;
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(RouteReader));
       
        string GetAccountList(IEnumerable<CarrierAccount> accounts)
        {
            StringBuilder sb = new StringBuilder("'NONE'");
            foreach (TABS.CarrierAccount account in accounts)
                sb.AppendFormat(",'{0}'", account.CarrierAccountID);
            
            return sb.ToString();
        }

        IDbCommand _routeCommand;
        IDbCommand _routeOptionsCommand;
        IDbConnection _connection;
        bool finished = false;
        // Last Synch?
        DateTime? _lastSynch;

         long _currentRouteID = 0;
        long _nextRouteID = 0;
         long _maxRouteID = 0;
        long _minRouteID = 1;

         List<Route> _routeList;
        IEnumerator<Route> _routeEnumerator;

        IDataParameter currentRouteIDParam1;
        IDataParameter nextRouteIDParam1;
        IDataParameter currentRouteIDParam2;
        IDataParameter nextRouteIDParam2;

        protected void SetRouteIDLimits(RouteSynchType synchType)
        {
            string additionalCondition = " AND CustomerID IN (" + _ValidCustomersIdList + ")";

            if (synchType == RouteSynchType.Full)
            {
                _maxRouteID = long.Parse(DataHelper.ExecuteScalar("SELECT ISNULL(MAX(RouteID),0) FROM [Route] WITH (NOLOCK) WHERE 1 = 1 " + additionalCondition).ToString());
            }
            else
            {
                _minRouteID = long.Parse(DataHelper.ExecuteScalar("SELECT ISNULL(MIN(RouteID),0) FROM [Route] WITH (NOLOCK) WHERE [Updated] >= @P1 " + additionalCondition, _lastSynch).ToString());
                _maxRouteID = long.Parse(DataHelper.ExecuteScalar("SELECT ISNULL(MAX(RouteID),0) FROM [Route] WITH (NOLOCK) WHERE [Updated] >= @P1 " + additionalCondition, _lastSynch).ToString());
            }
        }

        /// <summary>
        /// Create a reader for all Routes and Options
        /// </summary>
        public RouteReader()
            : this(CarrierAccount.Customers, CarrierAccount.Suppliers, 10000, RouteSynchType.Full, null)
        {

        }

        //private List<RouteInfo
        public RouteReader(IEnumerable<CarrierAccount> validCustomers, IEnumerable<CarrierAccount> validSuppliers, int batchSize, RouteSynchType synchType, Switch updatedSwitch)
        {
            this._ValidCustomersIdList = GetAccountList(validCustomers);
            this._ValidSuppliersIdList = GetAccountList(validSuppliers);
            
            this._BatchSize = batchSize < 1000 ? 1000 : batchSize;
            this._synchType = synchType;
            this._lastSynch = (synchType == RouteSynchType.Full) ? null : updatedSwitch.LastRouteUpdate;
            log.InfoFormat("Creating {0} reader for switch {1}. {2} customers and {3} suppliers. Batch size: {4}", synchType, updatedSwitch, validCustomers.Count(), validSuppliers.Count(), batchSize);

            _connection = DataHelper.GetOpenConnection();
            SetRouteIDLimits(synchType);
            
            _routeCommand = _connection.CreateCommand();
            _routeOptionsCommand = _connection.CreateCommand();

            _routeCommand.CommandText = string.Format(
                @"SELECT R.RouteID, R.CustomerID, R.Code, R.OurActiveRate 
                    FROM [Route] R WITH (NOLOCK {0}) 
                    WHERE 1=1 {1} -- Additional Conditions
                        AND R.CustomerID IN ({2})
                        AND R.RouteID >= @CurrentRouteID AND R.RouteID < @NextRouteID
                    ORDER BY R.RouteID
                    "
                , (_lastSynch.HasValue ? ", INDEX(IX_Route_Updated) " : "")
                , (_lastSynch.HasValue ? " AND R.Updated >= @LastSynch " : "")
                , this._ValidCustomersIdList
                );
            if (_lastSynch.HasValue) DataHelper.AddParameter(_routeCommand, "@LastSynch", _lastSynch.Value);

            if (synchType == RouteSynchType.Full)
            {
                _routeOptionsCommand.CommandText =
                    string.Format(
                        @"SELECT O.RouteID, O.SupplierID, O.Priority, O.NumberOfTries, O.SupplierActiveRate as RouteOptionState, O.Percentage as Percentage
                            FROM
                                [Route] R WITH(NOLOCK),
                                [RouteOption] O WITH (NOLOCK, INDEX(IDX_RouteOption_RouteID))
                            WHERE O.RouteID = R.RouteID
                                AND R.RouteID >= @CurrentRouteID AND R.RouteID < @NextRouteID
                                AND R.CustomerID IN ({0})
                                AND O.SupplierID IN ({1})
                                AND O.State = {2}",
                            this._ValidCustomersIdList,
                            this._ValidSuppliersIdList,
                            (int)RouteOptionState.Enabled);
            }
            else
            {
                _routeOptionsCommand.CommandText = string.Format(
                    @"SELECT O.RouteID, O.SupplierID, O.Priority, O.NumberOfTries, O.SupplierActiveRate, O.State as RouteOptionState, O.Percentage as Percentage                            FROM 
                                [Route] R WITH (NOLOCK, INDEX(IX_Route_Updated)), 
                                [RouteOption] O WITH (NOLOCK, INDEX(IDX_RouteOption_RouteID)) 
                            WHERE R.RouteID = O.RouteID 
                                AND R.RouteID >= @CurrentRouteID AND R.RouteID < @NextRouteID 
                                {0} -- Update Condition 
                                AND R.CustomerID IN ({1})
                                AND O.SupplierID IN ({2})
                                AND O.State = {3}"
                        , (_lastSynch.HasValue ? " AND R.Updated >= @LastSynch " : "")
                        , this._ValidCustomersIdList
                        , this._ValidSuppliersIdList
                        , (int)RouteOptionState.Enabled
                        );
                DataHelper.AddParameter(_routeOptionsCommand, "@LastSynch", _lastSynch.Value);
            }

            currentRouteIDParam1 = DataHelper.AddParameter(_routeCommand, "@CurrentRouteID", (long)1);
            nextRouteIDParam1 = DataHelper.AddParameter(_routeCommand, "@NextRouteID", (long)1);
            currentRouteIDParam2 = DataHelper.AddParameter(_routeOptionsCommand, "@CurrentRouteID", (long)1);
            nextRouteIDParam2 = DataHelper.AddParameter(_routeOptionsCommand, "@NextRouteID", (long)1);

            _currentRouteID = _minRouteID;
            _nextRouteID = _currentRouteID + _BatchSize;
        }

        #region IEnumerable<Route> Members

        public IEnumerator<Route> GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerator<Route> Members

        public Route Current
        {
            get { return (_routeEnumerator != null) ? _routeEnumerator.Current : null; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _connection.Dispose();
            _routeCommand.Dispose();
            _routeOptionsCommand.Dispose();
            _routeList = null;
            if (_routeEnumerator != null)
                _routeEnumerator.Dispose();
            _routeEnumerator = null;
            _totalRoutesRead = 0;
            _ValidCustomersIdList = null;
            _ValidSuppliersIdList = null;
            GC.Collect();
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return (_routeEnumerator != null) ? _routeEnumerator.Current : null; }
        }

        public void ReadNextBatch()
        {
            if (_currentRouteID <= _maxRouteID)
            {
                currentRouteIDParam1.Value = _currentRouteID;
                nextRouteIDParam1.Value = _nextRouteID;
                currentRouteIDParam2.Value = _currentRouteID;
                nextRouteIDParam2.Value = _nextRouteID;
                log.InfoFormat("Start Reading SQL Batch: {0} to {1}", _currentRouteID, _nextRouteID);

                // Read Routes
                log.InfoFormat("Start Reading Routes in  SQL Batch: {0} to {1}", _currentRouteID, _nextRouteID);
                using (IDataReader routeReader = _routeCommand.ExecuteReader())
                {
                    log.InfoFormat("End  Execute Routes Reader for  SQL Batch: {0} to {1}", _currentRouteID, _nextRouteID);
                    _routeList = null; 
                    _routeList = new List<Route>((int)_BatchSize);
                    Route.ReadRoutes(routeReader, _routeList);
                    routeReader.Close();
                    _totalRoutesRead += _routeList.Count;
                    log.InfoFormat("End Reading Routes in  SQL Batch: {0} to {1}", _currentRouteID, _nextRouteID);
                }

                // Read Options
                log.InfoFormat("Start Reading Routes Options in  SQL Batch: {0} to {1}", _currentRouteID, _nextRouteID);
                using (IDataReader routeOptionsReader = _routeOptionsCommand.ExecuteReader())
                {
                    var routeDictionary = _routeList.ToDictionary(r => r.ID);
                    Route.ReadOptions(routeOptionsReader, routeDictionary);
                    routeOptionsReader.Close();
                    log.InfoFormat("End Reading Routes Options in  SQL Batch: {0} to {1}", _currentRouteID, _nextRouteID);
                }
                log.InfoFormat("End Reading SQL Batch: {0} to {1}", _currentRouteID, _nextRouteID);
                _currentRouteID += _BatchSize;
                _nextRouteID += _BatchSize;
            }
            else
            {
                finished = true;
                if (_routeList == null)
                {
                    _routeList = new List<Route>((int)_BatchSize);
                }
                else
                {
                    _routeList.Clear();
                }
            }
            //GC.Collect();//recommanded for now the GC 
        }

        public bool MoveNext()
        {
            if (_routeList == null) ReadNextBatch();
            if (_routeEnumerator == null) _routeEnumerator = _routeList.GetEnumerator();
            bool more = _routeEnumerator.MoveNext();
            if (more) return true;
            else
            {
                if (finished)
                {
                    log.InfoFormat("Finished Reading. Total of {0} Routes Read", _totalRoutesRead);
                    return false;
                }
                else
                {
                    _routeList = null;
                    _routeEnumerator = null;
                    return MoveNext();
                }
            }
        }

        public void Reset()
        {
            _currentRouteID = _minRouteID;
            _nextRouteID = _currentRouteID + _BatchSize;
            _routeList = null;
            _routeEnumerator = null;
            _totalRoutesRead = 0;
            finished = false;
        }

        #endregion
    }
}