using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;
using Vanrise.Runtime;

namespace Vanrise.Integration.Adapters.SQLReceiveAdapter
{

    public class SQLReceiveAdapter : BaseReceiveAdapter
    {
        public override void ImportData(IAdapterImportDataContext context)
        {
            DBAdapterArgument dbAdapterArgument = context.AdapterArgument as DBAdapterArgument;
            if (dbAdapterArgument == null)
                throw new NullReferenceException("dbAdapterArgument");
            RunInParallelMode(context, dbAdapterArgument);
        }

        private void RunInParallelMode(IAdapterImportDataContext context, DBAdapterArgument dbAdapterArgument)
        {
            bool isLastRange;
            DBAdapterRangeState rangeToRead = GetAndLockNextRangeToRead(context, null, dbAdapterArgument, out isLastRange);

            if (rangeToRead == null)
            {
                LogInformation("No More Ranges to read");
                return;
            }

            string queryWithTop1 = dbAdapterArgument.Query.Replace("#TopRows#", "top 1");
            string queryWithNoTop = dbAdapterArgument.Query.Replace("#TopRows#", "");

            DBReaderImportedData data = null;
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(dbAdapterArgument.ConnectionString);
                connection.Open();

                var command = new SqlCommand(queryWithNoTop, connection);
                command.CommandTimeout = dbAdapterArgument.CommandTimeoutInSeconds != default(int) ? dbAdapterArgument.CommandTimeoutInSeconds : 600;
                DefineParameters(command, dbAdapterArgument);

                do
                {
                    try
                    {
                        Console.WriteLine("{0}: Reading Range {1} - {2}", DateTime.Now, rangeToRead.RangeStart, rangeToRead.RangeEnd);
                        ReadRange(context, dbAdapterArgument, isLastRange, rangeToRead, queryWithTop1, queryWithNoTop, ref data, command);
                    }
                    finally
                    {
                        ReleaseRange(context, rangeToRead);
                    }

                    rangeToRead = GetAndLockNextRangeToRead(context, rangeToRead, dbAdapterArgument, out isLastRange);
                    if (rangeToRead == null)
                        LogInformation("No More Ranges to read");
                }
                while (rangeToRead != null);
            }
            catch (Exception ex)
            {
                LogError("An error occurred in SQL Adapter while importing data. Exception Details: {0}", ex.ToString());
            }
            finally
            {
                if (data != null)
                    data.OnDisposed();
                if (connection != null)
                {
                    if (connection.State != System.Data.ConnectionState.Closed)
                        connection.Close();
                    connection.Dispose();
                }
            }
        }

        private void ReadRange(IAdapterImportDataContext context, DBAdapterArgument dbAdapterArgument, bool isLastRange, DBAdapterRangeState rangeToRead, string queryWithTop1, string queryWithNoTop, ref DBReaderImportedData data, SqlCommand command)
        {
            LogInformation("Started Reading Range {0} - {1}. Last Imported Id: {2}", rangeToRead.RangeStart, rangeToRead.RangeEnd, rangeToRead.LastImportedId);
            DateTime rangeReadStart = DateTime.Now;
            command.CommandText = queryWithNoTop;
            UpdateParametersValues(command, rangeToRead);

            if (rangeToRead.RangeEnd == null)
            {
                if (!TrySetRangeEnd(context, dbAdapterArgument, rangeToRead, queryWithTop1, queryWithNoTop, command))
                {
                    LogInformation("No Data Found In Range {0} - {1}", rangeToRead.RangeStart, rangeToRead.RangeEnd);
                    return;
                }
            }

            data = new DBReaderImportedData();
            data.Reader = command.ExecuteReader();

            if (!data.Reader.HasRows)//current range doesnt have data                    
            {
                data.OnDisposed();
                data = null;

                if (isLastRange)//rangeToRead.LastImportedId == null to ensure that no range can be created after it (by another runtime instance)
                {
                    OpenRangeEndAndTryGetData(context, dbAdapterArgument, rangeToRead, queryWithTop1, queryWithNoTop, ref data, command);
                }

                if (data == null)
                    return;
            }

            bool hasMoreRecords = false;
            bool anotherInstanceIsStarted = false;
            do
            {
                DateTime batchReadStart = DateTime.Now;

                data.LastImportedId = rangeToRead.LastImportedId;
                context.OnDataReceived(data);

                Object newLastImportedId = data.LastImportedId;

                LogBatchReadingProgress(rangeToRead, batchReadStart, newLastImportedId);

                if (newLastImportedId != null)
                {
                    hasMoreRecords = !newLastImportedId.Equals(rangeToRead.RangeEnd) && !newLastImportedId.Equals(rangeToRead.LastImportedId);
                    rangeToRead.LastImportedId = newLastImportedId;
                }

                context.GetStateWithLock((state) =>
                {
                    DBAdapterRangesState rangesState = state as DBAdapterRangesState;

                    DBAdapterRangeState matchCurrentRange = GetMatchRangeFromState(rangeToRead, rangesState);
                    matchCurrentRange.LastImportedId = rangeToRead.LastImportedId;
                    matchCurrentRange.RangeStart = rangeToRead.RangeStart;
                    matchCurrentRange.RangeEnd = rangeToRead.RangeEnd;
                    return state;
                });
                if (!anotherInstanceIsStarted)
                {
                    context.StartNewInstanceIfAllowed();
                    anotherInstanceIsStarted = true;
                }
            } while (hasMoreRecords);

            data.OnDisposed();
            data = null;

            LogInformation("Finished Reading Range {0} - {1} in {2}. Last Imported Id: {3}", rangeToRead.RangeStart, rangeToRead.RangeEnd, (DateTime.Now - rangeReadStart), rangeToRead.LastImportedId);
        }

        private bool TrySetRangeEnd(IAdapterImportDataContext context, DBAdapterArgument dbAdapterArgument, DBAdapterRangeState rangeToRead, string queryWithTop1, string queryWithNoTop, SqlCommand command)
        {
            command.CommandText = queryWithTop1;
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    rangeToRead.RangeEnd = reader[dbAdapterArgument.IdentifierColumnName];
                }
                reader.Close();
            }

            if (rangeToRead.RangeEnd != null)
            {
                command.CommandText = queryWithNoTop;
                UpdateParametersValues(command, rangeToRead);
                return true;
            }
            else
                return false;
        }

        private void OpenRangeEndAndTryGetData(IAdapterImportDataContext context, DBAdapterArgument dbAdapterArgument, DBAdapterRangeState rangeToRead, string queryWithTop1, string queryWithNoTop, ref DBReaderImportedData data, SqlCommand command)
        {
            command.CommandText = queryWithTop1;
            var originalRangeEnd = rangeToRead.RangeEnd;
            rangeToRead.RangeEnd = null;
            UpdateParametersValues(command, rangeToRead);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    rangeToRead.RangeEnd = reader[dbAdapterArgument.IdentifierColumnName];
                }
                reader.Close();
            }

            if (rangeToRead.RangeEnd != null)
            {
                //ensure that the current range is the last range and update it in the adapter state
                context.GetStateWithLock((state) =>
                {
                    DBAdapterRangesState rangesState = state as DBAdapterRangesState;
                    var matchRange = GetMatchRangeFromState(rangeToRead, rangesState);
                    if (rangesState.Ranges.IndexOf(matchRange) == rangesState.Ranges.Count - 1)//last range
                        matchRange.RangeEnd = rangeToRead.RangeEnd;
                    else
                        rangeToRead.RangeEnd = null;
                    return state;
                });
            }

            if (rangeToRead.RangeEnd != null)
            {
                
                command.CommandText = queryWithNoTop;
                UpdateParametersValues(command, rangeToRead);

                data = new DBReaderImportedData();
                data.Reader = command.ExecuteReader();
                if (!data.Reader.HasRows)
                {
                    data.OnDisposed();
                    data = null;
                }
            }

            if (data == null)
                rangeToRead.RangeEnd = originalRangeEnd;
        }

        private void LogBatchReadingProgress(DBAdapterRangeState rangeToRead, DateTime batchReadStart, object newLastImportedId)
        {
            if (newLastImportedId == null || newLastImportedId == DBNull.Value)
                LogError("LastImportedId is Null in Range {0} - {1}. Original LastImportedId '{2}'", rangeToRead.RangeStart, rangeToRead.RangeEnd, rangeToRead.LastImportedId);
            else if (!newLastImportedId.Equals(rangeToRead.LastImportedId))
                LogInformation("Batch Read From Range {0} - {1} in {2}. Original LastImportedId '{3}', new LastImportedId '{4}'", rangeToRead.RangeStart, rangeToRead.RangeEnd, (DateTime.Now - batchReadStart), rangeToRead.LastImportedId, newLastImportedId);
            else
                LogWarning("Batch Read From Range {0} - {1} in {2}. LastImportedId is not changed '{3}'", rangeToRead.RangeStart, rangeToRead.RangeEnd, (DateTime.Now - batchReadStart), newLastImportedId);
        }

        private void ReleaseRange(IAdapterImportDataContext context, DBAdapterRangeState range)
        {
            context.GetStateWithLock((state) =>
            {
                DBAdapterRangesState rangesState = state as DBAdapterRangesState;

                DBAdapterRangeState matchCurrentRange = GetMatchRangeFromState(range, rangesState);
                range = matchCurrentRange;
                range.LockedByProcessId = null;
                var indexOfCurrentRange = rangesState.Ranges.IndexOf(range);
                if (indexOfCurrentRange != rangesState.Ranges.Count - 1)//not last range
                {
                    if (range.LastImportedId != null && range.LastImportedId.Equals(range.RangeEnd))
                    {
                        rangesState.Ranges.Remove(range);
                    }
                    else
                    {
                        for (int i = indexOfCurrentRange + 1; i < rangesState.Ranges.Count; i++)
                        {
                            if (rangesState.Ranges[i].LastImportedId != null)//if data is found in a range that is after current range
                            {
                                rangesState.Ranges.Remove(range);
                                break;
                            }
                        }
                    }
                }

                return state;
            });
            
        }

        private void DefineParameters(SqlCommand command, DBAdapterArgument dbAdapterArgument)
        {
            command.Parameters.Add("@RangeStart", GetRangePrmSQLType(dbAdapterArgument));
            command.Parameters.Add("@RangeEnd", GetRangePrmSQLType(dbAdapterArgument));
        }

        private void UpdateParametersValues(SqlCommand command, DBAdapterRangeState rangeToRead)
        {
            SetSQLParameterValue(command.Parameters["@RangeStart"], rangeToRead.LastImportedId != null ? rangeToRead.LastImportedId : rangeToRead.RangeStart);
            SetSQLParameterValue(command.Parameters["@RangeEnd"], rangeToRead.RangeEnd);
        }

        private System.Data.SqlDbType GetRangePrmSQLType(DBAdapterArgument dbAdapterArgument)
        {
            if (dbAdapterArgument.NumberOffSet.HasValue)
                return System.Data.SqlDbType.BigInt;
            else if (dbAdapterArgument.TimeOffset.HasValue)
                return System.Data.SqlDbType.DateTime;
            else
                throw new Exception("DBAdapterArgument doesnt have any Offset defined");
        }

        void SetSQLParameterValue(SqlParameter prm, object value)
        {
            if (value == null)
                prm.Value = DBNull.Value;
            else
                prm.Value = value;
        }

        private DBAdapterRangeState GetMatchRangeFromState(DBAdapterRangeState range, DBAdapterRangesState rangesState)
        {
            return rangesState.Ranges.FirstOrDefault(itm => itm.RangeId == range.RangeId);
        }

        private DBAdapterRangeState GetAndLockNextRangeToRead(IAdapterImportDataContext context, DBAdapterRangeState currentRange, DBAdapterArgument dbAdapterArgument, out bool isLastRange)
        {
            DBAdapterRangeState rangeToRead = null;

            bool islastRange_local = false;

            context.GetStateWithLock((state) =>
            {
                DBAdapterRangesState rangesState = state as DBAdapterRangesState;
                if (rangesState == null)
                    rangesState = new DBAdapterRangesState { Ranges = new List<DBAdapterRangeState>() };

                RunningProcessManager runningProcessManager = new RunningProcessManager();
                IEnumerable<int> runningRuntimeProcessesIds = runningProcessManager.GetCachedRunningProcesses().Select(itm => itm.ProcessId);
                int startingIndex = 0;
                if(currentRange != null)
                {
                    var matchCurrentRange = GetMatchRangeFromState(currentRange, rangesState);
                    if (matchCurrentRange != null)
                        startingIndex = rangesState.Ranges.IndexOf(matchCurrentRange) + 1;
                }
                for (int i = startingIndex; i < rangesState.Ranges.Count; i++)
                {
                    var range = rangesState.Ranges[i];
                    if (!range.LockedByProcessId.HasValue || !runningRuntimeProcessesIds.Contains(range.LockedByProcessId.Value))
                    {
                        rangeToRead = range;
                        if (i == rangesState.Ranges.Count - 1)
                            islastRange_local = true;
                        break;
                    }
                }

                if (rangeToRead == null)
                {
                    DBAdapterRangeState lastRange = rangesState.Ranges.LastOrDefault();
                    if(lastRange == null //no ranges yet (first time import)
                            ||
                            lastRange.LastImportedId != null)//last range has imported data
                    {                        
                        rangeToRead = CreateRange(dbAdapterArgument, rangesState);
                        rangesState.Ranges.Add(rangeToRead);
                        islastRange_local = true;
                    }
                }

                if (rangeToRead != null)
                {
                    rangeToRead.LockedByProcessId = RunningProcessManager.CurrentProcess.ProcessId;
                }

                return rangesState;
            });
            isLastRange = islastRange_local;
            return rangeToRead;
        }

        private DBAdapterRangeState CreateRange(DBAdapterArgument dbAdapterArgument, DBAdapterRangesState rangesState)
        {
            DBAdapterRangeState lastRange = rangesState.Ranges.LastOrDefault();
            DBAdapterRangeState newRange = new DBAdapterRangeState() { RangeId = Guid.NewGuid() };
            if (lastRange != null)
            {
                newRange.RangeStart = lastRange.RangeEnd;
                SetRangeEnd(newRange, dbAdapterArgument);
            }
            return newRange;
        }

        private static void SetRangeEnd(DBAdapterRangeState range, DBAdapterArgument dbAdapterArgument)
        {
            if (dbAdapterArgument.NumberOffSet.HasValue)
                range.RangeEnd = Convert.ToInt64(range.RangeStart) + dbAdapterArgument.NumberOffSet.Value;
            else if (dbAdapterArgument.TimeOffset.HasValue)
                range.RangeEnd = Convert.ToDateTime(range.RangeStart).Add(dbAdapterArgument.TimeOffset.Value);
            else
                throw new Exception("DBAdapterArgument doesnt have any Offset defined");
        }

        private void RunInSingleMode(IAdapterImportDataContext context, DBAdapterArgument dbAdapterArgument)
        {
            throw new NotImplementedException();
        }
    }
}