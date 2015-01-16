using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

namespace TABS.Components
{
    public class BulkManager : IDisposable
    {
        protected log4net.ILog log;
        public SqlConnection Connection { get; protected set; }

        protected static DataTable _BillingCdrTable, _PricingTable, _BillingStatsTable, _TrafficStatsTable, _CdrTable, _TrafficStatsDailyTable, _TrafficStatsByOriginationDaily, _CodeTable, _Zone, _TrafficStatsTempTable;

        protected static DataTable CodeTable
        {
            get
            {
                lock (typeof(BulkManager))
                {
                    if (_CodeTable == null)
                    {
                        _CodeTable = new DataTable();
                        _CodeTable.Columns.Add("Code", typeof(string));
                        _CodeTable.Columns.Add("ZoneID", typeof(string));
                        _CodeTable.Columns.Add("BeginEffectiveDate", typeof(DateTime));
                        _CodeTable.Columns.Add("EndEffectiveDate", typeof(DateTime));
                        _CodeTable.Columns.Add("UserID", typeof(int));
                    }
                }
                return _CodeTable;
            }
        }


        protected static DataTable TrafficStatsTable
        {
            get
            {
                lock (typeof(BulkManager))
                {
                    if (_TrafficStatsTable == null)
                    {
                        _TrafficStatsTable = new DataTable();
                        _TrafficStatsTable.Columns.Add("SwitchId", typeof(int));
                        _TrafficStatsTable.Columns.Add("Port_IN", typeof(string));
                        _TrafficStatsTable.Columns.Add("Port_OUT", typeof(string));
                        _TrafficStatsTable.Columns.Add("CustomerID", typeof(string));
                        _TrafficStatsTable.Columns.Add("OurZoneID", typeof(int));
                        _TrafficStatsTable.Columns.Add("OriginatingZoneID", typeof(int));
                        _TrafficStatsTable.Columns.Add("SupplierID", typeof(string));
                        _TrafficStatsTable.Columns.Add("SupplierZoneID", typeof(int));
                        _TrafficStatsTable.Columns.Add("FirstCDRAttempt", typeof(DateTime));
                        _TrafficStatsTable.Columns.Add("LastCDRAttempt", typeof(DateTime));
                        _TrafficStatsTable.Columns.Add("Attempts", typeof(int));
                        _TrafficStatsTable.Columns.Add("DeliveredAttempts", typeof(int));
                        _TrafficStatsTable.Columns.Add("SuccessfulAttempts", typeof(int));
                        _TrafficStatsTable.Columns.Add("DurationsInSeconds", typeof(decimal));
                        _TrafficStatsTable.Columns.Add("PDDInSeconds", typeof(decimal));
                        _TrafficStatsTable.Columns.Add("MaxDurationInSeconds", typeof(decimal));
                        _TrafficStatsTable.Columns.Add("UtilizationInSeconds", typeof(decimal));
                        _TrafficStatsTable.Columns.Add("NumberOfCalls", typeof(int));
                        _TrafficStatsTable.Columns.Add("DeliveredNumberOfCalls", typeof(int));
                        _TrafficStatsTable.Columns.Add("PGAD", typeof(int));
                        _TrafficStatsTable.Columns.Add("CeiledDuration", typeof(int));
                        _TrafficStatsTable.Columns.Add("ReleaseSourceAParty", typeof(int));
                    }
                }
                return _TrafficStatsTable;
            }
        }


        protected static DataTable TrafficStatsTempTable
        {
            get
            {
                lock (typeof(BulkManager))
                {
                    if (_TrafficStatsTempTable == null)
                    {
                        _TrafficStatsTempTable = new DataTable();
                        _TrafficStatsTempTable.Columns.Add("ID", typeof(long));
                        _TrafficStatsTempTable.Columns.Add("SwitchId", typeof(int));
                        _TrafficStatsTempTable.Columns.Add("Port_IN", typeof(string));
                        _TrafficStatsTempTable.Columns.Add("Port_OUT", typeof(string));
                        _TrafficStatsTempTable.Columns.Add("CustomerID", typeof(string));
                        _TrafficStatsTempTable.Columns.Add("OurZoneID", typeof(int));
                        _TrafficStatsTempTable.Columns.Add("OriginatingZoneID", typeof(int));
                        _TrafficStatsTempTable.Columns.Add("SupplierID", typeof(string));
                        _TrafficStatsTempTable.Columns.Add("SupplierZoneID", typeof(int));
                        _TrafficStatsTempTable.Columns.Add("FirstCDRAttempt", typeof(DateTime));
                        _TrafficStatsTempTable.Columns.Add("LastCDRAttempt", typeof(DateTime));
                        _TrafficStatsTempTable.Columns.Add("Attempts", typeof(int));
                        _TrafficStatsTempTable.Columns.Add("DeliveredAttempts", typeof(int));
                        _TrafficStatsTempTable.Columns.Add("SuccessfulAttempts", typeof(int));
                        _TrafficStatsTempTable.Columns.Add("DurationsInSeconds", typeof(decimal));
                        _TrafficStatsTempTable.Columns.Add("PDDInSeconds", typeof(decimal));
                        _TrafficStatsTempTable.Columns.Add("MaxDurationInSeconds", typeof(decimal));
                        _TrafficStatsTempTable.Columns.Add("UtilizationInSeconds", typeof(decimal));
                        _TrafficStatsTempTable.Columns.Add("NumberOfCalls", typeof(int));
                        _TrafficStatsTempTable.Columns.Add("DeliveredNumberOfCalls", typeof(int));
                        _TrafficStatsTempTable.Columns.Add("PGAD", typeof(int));
                        _TrafficStatsTempTable.Columns.Add("CeiledDuration", typeof(int));
                        _TrafficStatsTempTable.Columns.Add("ReleaseSourceAParty", typeof(int));
                    }
                }
                return _TrafficStatsTempTable;
            }
        }

        protected static DataTable BillingCdrTable
        {
            get
            {
                lock (typeof(BulkManager))
                {
                    if (_BillingCdrTable == null)
                    {
                        _BillingCdrTable = new DataTable();
                        _BillingCdrTable.Columns.Add("ID", typeof(long));
                        _BillingCdrTable.Columns.Add("Attempt", typeof(DateTime));
                        _BillingCdrTable.Columns.Add("Alert", typeof(DateTime));
                        _BillingCdrTable.Columns.Add("Connect", typeof(DateTime));
                        _BillingCdrTable.Columns.Add("Disconnect", typeof(DateTime));
                        _BillingCdrTable.Columns.Add("DurationInSeconds", typeof(decimal));
                        _BillingCdrTable.Columns.Add("CustomerID", typeof(string));
                        _BillingCdrTable.Columns.Add("OurZoneID", typeof(int));
                        _BillingCdrTable.Columns.Add("OriginatingZoneID", typeof(int));
                        _BillingCdrTable.Columns.Add("SupplierID", typeof(string));
                        _BillingCdrTable.Columns.Add("SupplierZoneID", typeof(int));
                        _BillingCdrTable.Columns.Add("CDPN", typeof(string));
                        _BillingCdrTable.Columns.Add("CGPN", typeof(string));
                        _BillingCdrTable.Columns.Add("CDPNOut", typeof(string));
                        _BillingCdrTable.Columns.Add("ReleaseCode", typeof(string));
                        _BillingCdrTable.Columns.Add("ReleaseSource", typeof(string));
                        _BillingCdrTable.Columns.Add("SwitchID", typeof(byte));
                        _BillingCdrTable.Columns.Add("SwitchCdrID", typeof(long));
                        _BillingCdrTable.Columns.Add("Tag", typeof(string));
                        _BillingCdrTable.Columns.Add("Extra_Fields", typeof(string));
                        _BillingCdrTable.Columns.Add("Port_IN", typeof(string));
                        _BillingCdrTable.Columns.Add("Port_OUT", typeof(string));
                        _BillingCdrTable.Columns.Add("OurCode", typeof(string));
                        _BillingCdrTable.Columns.Add("SupplierCode", typeof(string));
                        _BillingCdrTable.Columns.Add("IsRerouted", typeof(string));
                    }
                }
                return _BillingCdrTable;
            }
        }

        protected static DataTable CdrTable
        {
            get
            {
                lock (typeof(BulkManager))
                {
                    if (_CdrTable == null)
                    {
                        _CdrTable = new DataTable();
                        _CdrTable.Columns.Add("SwitchID", typeof(byte));
                        _CdrTable.Columns.Add("IDonSwitch", typeof(long));
                        _CdrTable.Columns.Add("Tag", typeof(string));
                        _CdrTable.Columns.Add("AttemptDateTime", typeof(DateTime));
                        _CdrTable.Columns.Add("AlertDateTime", typeof(DateTime));
                        _CdrTable.Columns.Add("ConnectDateTime", typeof(DateTime));
                        _CdrTable.Columns.Add("DisconnectDateTime", typeof(DateTime));
                        _CdrTable.Columns.Add("DurationInSeconds", typeof(decimal));
                        _CdrTable.Columns.Add("IN_TRUNK", typeof(string));
                        _CdrTable.Columns.Add("IN_CIRCUIT", typeof(short));
                        _CdrTable.Columns.Add("IN_CARRIER", typeof(string));
                        _CdrTable.Columns.Add("IN_IP", typeof(string));
                        _CdrTable.Columns.Add("OUT_TRUNK", typeof(string));
                        _CdrTable.Columns.Add("OUT_CIRCUIT", typeof(short));
                        _CdrTable.Columns.Add("OUT_CARRIER", typeof(string));
                        _CdrTable.Columns.Add("OUT_IP", typeof(string));
                        _CdrTable.Columns.Add("CGPN", typeof(string));
                        _CdrTable.Columns.Add("CDPN", typeof(string));
                        _CdrTable.Columns.Add("CDPNOut", typeof(string));
                        _CdrTable.Columns.Add("CAUSE_FROM", typeof(string));
                        _CdrTable.Columns.Add("CAUSE_TO", typeof(string));
                        _CdrTable.Columns.Add("CAUSE_FROM_RELEASE_CODE", typeof(string));
                        _CdrTable.Columns.Add("CAUSE_TO_RELEASE_CODE", typeof(string));
                        _CdrTable.Columns.Add("Extra_Fields", typeof(string));
                        _CdrTable.Columns.Add("IsRerouted", typeof(string));
                        //_CdrTable.Columns.Add("SIP", typeof(string));
                    }
                }
                return _CdrTable;
            }
        }

        protected static DataTable BillingStatsTable
        {
            get
            {
                lock (typeof(BulkManager))
                {
                    if (_BillingStatsTable == null)
                    {
                        _BillingStatsTable = new DataTable();
                        _BillingStatsTable.Columns.Add("CallDate", typeof(DateTime));
                        _BillingStatsTable.Columns.Add("CustomerID", typeof(string));
                        _BillingStatsTable.Columns.Add("SupplierID", typeof(string));
                        _BillingStatsTable.Columns.Add("CostZoneID", typeof(int));
                        _BillingStatsTable.Columns.Add("SaleZoneID", typeof(int));
                        _BillingStatsTable.Columns.Add("Cost_Currency", typeof(string));
                        _BillingStatsTable.Columns.Add("Sale_Currency", typeof(string));
                        _BillingStatsTable.Columns.Add("SaleDuration", typeof(decimal));
                        _BillingStatsTable.Columns.Add("CostDuration", typeof(decimal));
                        _BillingStatsTable.Columns.Add("NumberOfCalls", typeof(int));
                        _BillingStatsTable.Columns.Add("FirstCallTime", typeof(string));
                        _BillingStatsTable.Columns.Add("LastCallTime", typeof(string));
                        _BillingStatsTable.Columns.Add("MinDuration", typeof(decimal));
                        _BillingStatsTable.Columns.Add("MaxDuration", typeof(decimal));
                        _BillingStatsTable.Columns.Add("AvgDuration", typeof(decimal));
                        _BillingStatsTable.Columns.Add("Cost_Nets", typeof(float));
                        _BillingStatsTable.Columns.Add("Cost_Discounts", typeof(float));
                        _BillingStatsTable.Columns.Add("Cost_Commissions", typeof(float));
                        _BillingStatsTable.Columns.Add("Cost_ExtraCharges", typeof(float));
                        _BillingStatsTable.Columns.Add("Sale_Nets", typeof(float));
                        _BillingStatsTable.Columns.Add("Sale_Discounts", typeof(float));
                        _BillingStatsTable.Columns.Add("Sale_Commissions", typeof(float));
                        _BillingStatsTable.Columns.Add("Sale_ExtraCharges", typeof(float));
                        _BillingStatsTable.Columns.Add("Sale_Rate", typeof(float));
                        _BillingStatsTable.Columns.Add("Cost_Rate", typeof(float));
                    }
                }
                return _BillingStatsTable;
            }
        }

        protected static DataTable PricingTable
        {
            get
            {
                lock (typeof(BulkManager))
                {
                    if (_PricingTable == null)
                    {
                        _PricingTable = new DataTable();
                        _PricingTable.Columns.Add("ID", typeof(long));
                        _PricingTable.Columns.Add("ZoneID", typeof(int));
                        _PricingTable.Columns.Add("Net", typeof(float));
                        _PricingTable.Columns.Add("CurrencyID", typeof(string));
                        _PricingTable.Columns.Add("RateValue", typeof(float));
                        _PricingTable.Columns.Add("RateID", typeof(long));
                        _PricingTable.Columns.Add("Discount", typeof(float));
                        _PricingTable.Columns.Add("RateType", typeof(byte));
                        _PricingTable.Columns.Add("ToDConsiderationID", typeof(long));
                        _PricingTable.Columns.Add("FirstPeriod", typeof(float));
                        _PricingTable.Columns.Add("RepeatFirstperiod", typeof(byte));
                        _PricingTable.Columns.Add("FractionUnit", typeof(byte));
                        _PricingTable.Columns.Add("TariffID", typeof(long));
                        _PricingTable.Columns.Add("CommissionValue", typeof(float));
                        _PricingTable.Columns.Add("CommissionID", typeof(int));
                        _PricingTable.Columns.Add("ExtraChargeValue", typeof(float));
                        _PricingTable.Columns.Add("ExtraChargeID", typeof(int));
                        _PricingTable.Columns.Add("Updated", typeof(DateTime));
                        _PricingTable.Columns.Add("DurationInSeconds", typeof(decimal));
                        _PricingTable.Columns.Add("Code", typeof(string));
                        _PricingTable.Columns.Add("Attempt", typeof(DateTime));

                    }
                }
                return _PricingTable;
            }
        }

        /// <summary>
        /// Write the given Traffic Stats to the database
        /// </summary>
        /// <param name="stats"></param>
        public void Write(IEnumerable<TrafficStats> stats)
        {
            lock (typeof(TrafficStats))
            {
                using (SqlTransaction transaction = this.Connection.BeginTransaction())
                {
                    try
                    {
                        DataTable _Stats = GetTrafficStatsTable(stats, "TrafficStats");
                        Write(_Stats, transaction, SqlBulkCopyOptions.KeepNulls);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        try { transaction.Rollback(); }
                        catch { }
                        log.Error("Error Performing Bulk Operation for Traffic Stats", ex);
                        throw (ex);
                    }
                }
            }
        }

        public void WriteAndUpdateTrafficStats(IEnumerable<TrafficStats> stats, out Exception exc)
        {
            exc = null;
            lock (typeof(TrafficStats))
            {

                string CreateTableQuery = @"CREATE TABLE [#TrafficStatsTemp]
                                (
                                    [ID] [bigint],
                                    [SwitchId] [tinyint],
	                                [Port_IN] [varchar](42),
	                                [Port_OUT] [varchar](42),
	                                [CustomerID] [varchar](10),
	                                [OurZoneID] [int],
	                                [OriginatingZoneID] [int],
	                                [SupplierID] [varchar](10),
	                                [SupplierZoneID] [int],
	                                [FirstCDRAttempt] [datetime],
	                                [LastCDRAttempt] [datetime],
	                                [Attempts] [int],
	                                [DeliveredAttempts] [int],
	                                [SuccessfulAttempts] [int],
	                                [DurationsInSeconds] [numeric](19, 5),
	                                [PDDInSeconds] [numeric](19, 5),
	                                [MaxDurationInSeconds] [numeric](19, 5),
	                                [UtilizationInSeconds] [numeric](19, 5),
	                                [NumberOfCalls] [int],
	                                [DeliveredNumberOfCalls] [int],
                                    [PGAD] [int],
                                    [CeiledDuration] [bigint],
                                    [ReleaseSourceAParty] [int]
                                )";

                string CreateIDMapQuery = @"CREATE TABLE [#TrafficStats_Surrogate_Map]
                                     (
                                          [VirtualID] [bigint],
                                          [InsertedID] [bigint]
                                     )";

                string UpdateInsertQuery = @"UPDATE T SET T.[FirstCDRAttempt] = SRC.[FirstCDRAttempt],
                                              T.[LastCDRAttempt] = SRC.[LastCDRAttempt],
                                              T.[Attempts] = SRC.[Attempts],
                                              T.[DeliveredAttempts] = SRC.[DeliveredAttempts],
                                              T.[SuccessfulAttempts] = SRC.[SuccessfulAttempts],
                                              T.[DurationsInSeconds] = SRC.[DurationsInSeconds],
                                              T.[PDDInSeconds] = SRC.[PDDInSeconds],
                                              T.[MaxDurationInSeconds] = SRC.[MaxDurationInSeconds],
                                              T.[UtilizationInSeconds] = SRC.[UtilizationInSeconds],
                                              T.[NumberOfCalls] = SRC.[NumberOfCalls],
                                              T.[DeliveredNumberOfCalls]  = SRC.[DeliveredNumberOfCalls],
                                              T.[PGAD] = SRC.[PGAD],
                                              T.[CeiledDuration] = SRC.[CeiledDuration],
                                              T.[ReleaseSourceAParty] = SRC.[ReleaseSourceAParty]
                                FROM [TrafficStats] T,[#TrafficStatsTemp] SRC
                                WHERE (T.[ID] = SRC.[ID] AND SRC.[ID] > 0);


                            DECLARE @ID bigint,
                                    @SwitchId tinyint,
	                                @Port_IN varchar(42),
	                                @Port_OUT varchar(42),
	                                @CustomerID varchar(10),
	                                @OurZoneID int,
	                                @OriginatingZoneID int,
	                                @SupplierID varchar(10),
	                                @SupplierZoneID int,
	                                @FirstCDRAttempt datetime,
	                                @LastCDRAttempt datetime,
	                                @Attempts int,
	                                @DeliveredAttempts int,
	                                @SuccessfulAttempts int,
	                                @DurationsInSeconds numeric(19, 5),
	                                @PDDInSeconds numeric(19, 5),
	                                @MaxDurationInSeconds numeric(19, 5),
	                                @UtilizationInSeconds numeric(19, 5),
	                                @NumberOfCalls int,
	                                @DeliveredNumberOfCalls int,
                                    @PGAD int,
                                    @CeiledDuration bigint,
                                    @ReleaseSourceAParty int
									
		                                 DECLARE temp_cursor CURSOR FOR
		                                 SELECT SRC.[ID],
												SRC.[SwitchId],
	                                            SRC.[Port_IN],
	                                            SRC.[Port_OUT],
	                                            SRC.[CustomerID],
	                                            SRC.[OurZoneID],
	                                            SRC.[OriginatingZoneID],
	                                            SRC.[SupplierID],
	                                            SRC.[SupplierZoneID],
	                                            SRC.[FirstCDRAttempt],
	                                            SRC.[LastCDRAttempt],
	                                            SRC.[Attempts],
	                                            SRC.[DeliveredAttempts],
	                                            SRC.[SuccessfulAttempts],
	                                            SRC.[DurationsInSeconds],
	                                            SRC.[PDDInSeconds],
	                                            SRC.[MaxDurationInSeconds],
	                                            SRC.[UtilizationInSeconds],
	                                            SRC.[NumberOfCalls],
	                                            SRC.[DeliveredNumberOfCalls],
                                                SRC.[PGAD],
                                                SRC.[CeiledDuration],
                                                SRC.[ReleaseSourceAParty]
                                                FROM [#TrafficStatsTemp] SRC WHERE SRC.[ID] <= -2;
      
		                                  OPEN temp_cursor
      
		                                FETCH NEXT FROM temp_cursor INTO 
		                                @ID,
										@SwitchId,
										@Port_IN,
										@Port_OUT,
										@CustomerID,
										@OurZoneID,
										@OriginatingZoneID,
										@SupplierID,
										@SupplierZoneID,
										@FirstCDRAttempt,
										@LastCDRAttempt,
										@Attempts,
										@DeliveredAttempts,
										@SuccessfulAttempts,
										@DurationsInSeconds,
										@PDDInSeconds,
										@MaxDurationInSeconds,
										@UtilizationInSeconds,
										@NumberOfCalls,
										@DeliveredNumberOfCalls,
										@PGAD,
                                        @CeiledDuration,
                                        @ReleaseSourceAParty
      
		                                  WHILE @@FETCH_STATUS = 0
		                                  BEGIN
		
								INSERT INTO [TrafficStats]
												(
													[SwitchId],
													[Port_IN],
													[Port_OUT],
													[CustomerID],
													[OurZoneID],
													[OriginatingZoneID],
													[SupplierID],
													[SupplierZoneID],
													[FirstCDRAttempt],
													[LastCDRAttempt],
													[Attempts],
													[DeliveredAttempts],
													[SuccessfulAttempts],
													[DurationsInSeconds],
													[PDDInSeconds],
													[MaxDurationInSeconds],
													[UtilizationInSeconds],
													[NumberOfCalls],
													[DeliveredNumberOfCalls],
													[PGAD],
                                                    [CeiledDuration],
                                                    [ReleaseSourceAParty]    
												)
												OUTPUT @ID,INSERTED.ID
												INTO [#TrafficStats_Surrogate_Map]
												VALUES
												(
													@SwitchId,
													@Port_IN,
													@Port_OUT,
													@CustomerID,
													@OurZoneID,
													@OriginatingZoneID,
													@SupplierID,
													@SupplierZoneID,
													@FirstCDRAttempt,
													@LastCDRAttempt,
													@Attempts,
													@DeliveredAttempts,
													@SuccessfulAttempts,
													@DurationsInSeconds,
													@PDDInSeconds,
													@MaxDurationInSeconds,
													@UtilizationInSeconds,
													@NumberOfCalls,
													@DeliveredNumberOfCalls,
													@PGAD,
                                                    @CeiledDuration,
                                                    @ReleaseSourceAParty
												)
		
		                                     FETCH NEXT FROM temp_cursor INTO 
												@ID,
												@SwitchId,
												@Port_IN,
												@Port_OUT,
												@CustomerID,
												@OurZoneID,
												@OriginatingZoneID,
												@SupplierID,
												@SupplierZoneID,
												@FirstCDRAttempt,
												@LastCDRAttempt,
												@Attempts,
												@DeliveredAttempts,
												@SuccessfulAttempts,
												@DurationsInSeconds,
												@PDDInSeconds,
												@MaxDurationInSeconds,
												@UtilizationInSeconds,
												@NumberOfCalls,
												@DeliveredNumberOfCalls,
												@PGAD,
                                                @CeiledDuration,
                                                @ReleaseSourceAParty
		                                  END
		                                  CLOSE temp_cursor
		                                  DEALLOCATE temp_cursor;";

                string SelectIdMap = @"SELECT * FROM #TrafficStats_Surrogate_Map";

                using (SqlTransaction transaction = this.Connection.BeginTransaction())
                {
                    try
                    {
                        int i = -2;

                        foreach (var stat in stats.Where(s => s.ID == 0))
                        {
                            stat.ID = i--;
                        }

                        DataTable _Stats = GetTrafficStatsTempTable(stats, "[#TrafficStatsTemp]");
                        SqlCommand Command = new SqlCommand(CreateTableQuery, this.Connection, transaction);
                        Command.ExecuteNonQuery();
                        Command.CommandText = CreateIDMapQuery;
                        Command.ExecuteNonQuery();
                        Write(_Stats, transaction, SqlBulkCopyOptions.KeepNulls);
                        Command.CommandText = UpdateInsertQuery;
                        Command.ExecuteNonQuery();
                        Command.CommandText = SelectIdMap;
                        var TrafficStatsByID = stats.ToDictionary(s => s.ID);
                        using (var reader = Command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                long VirtualID = reader.GetInt64(0);
                                long InsertedID = reader.GetInt64(1);
                                TrafficStatsByID[VirtualID].ID = InsertedID;
                            }
                        }
                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        log.Error("Error Performing Bulk Save/Update for Traffic Stats, Rolling Back Transaction", ex);
                        exc = ex;
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception e)
                        {
                            exc = e;
                            log.Error("Traffic Stats Bulk Save/Update Transaction Rollback Failed");
                        }

                    }
                }
            }
        }

        protected static DataTable TrafficStatsDailyTable
        {
            get
            {
                lock (typeof(BulkManager))
                {
                    if (_TrafficStatsDailyTable == null)
                    {
                        _TrafficStatsDailyTable = new DataTable();
                        _TrafficStatsDailyTable.Columns.Add("ID", typeof(long));
                        _TrafficStatsDailyTable.Columns.Add("CallDate", typeof(DateTime));
                        _TrafficStatsDailyTable.Columns.Add("SwitchID", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("CustomerID", typeof(string));
                        _TrafficStatsDailyTable.Columns.Add("OurZoneID", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("OriginatingZoneID", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("SupplierID", typeof(string));
                        _TrafficStatsDailyTable.Columns.Add("SupplierZoneID", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("Attempts", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("DeliveredAttempts", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("SuccessfulAttempts", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("DurationsInSeconds", typeof(decimal));
                        _TrafficStatsDailyTable.Columns.Add("PDDInSeconds", typeof(decimal));
                        _TrafficStatsDailyTable.Columns.Add("UtilizationInSeconds", typeof(decimal));
                        _TrafficStatsDailyTable.Columns.Add("NumberOfCalls", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("DeliveredNumberOfCalls", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("PGAD", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("CeiledDuration", typeof(int));
                        _TrafficStatsDailyTable.Columns.Add("MaxDurationInSeconds", typeof(decimal));
                        _TrafficStatsDailyTable.Columns.Add("ReleaseSourceAParty", typeof(int));
                    }
                }
                return _TrafficStatsDailyTable;
            }
            set { _TrafficStatsDailyTable = value; }
        }


        protected DataTable GetDailyTrafficStatsTable(IEnumerable<TrafficStats> stats, string tableName)
        {
            log.DebugFormat("Bulk Building Data table: {0}", tableName);
            DataTable DailystatsTable = TrafficStatsDailyTable.Clone();
            DailystatsTable.TableName = tableName;
            DailystatsTable.BeginLoadData();
            object MaxIDO = TABS.DataHelper.ExecuteScalar("SELECT MAX(ID) AS MaxID FROM TrafficStatsDaily tsd", null);
            long maxID = 0;
            if (MaxIDO.ToString() != string.Empty)
                maxID = (long)MaxIDO;
            maxID += 1;
            foreach (var group in stats)//.GroupBy(s=>s.CallDate.ToString("yyyy-MM-dd")
            {
                DataRow row = DailystatsTable.NewRow();
                int index = -1;
                index++; row[index] = maxID; maxID += 1;
                index++; row[index] = group.CallDate;
                index++; row[index] = group.Switch.SwitchID;
                index++; row[index] = group.Customer == null ? DBNull.Value : (object)group.Customer.CarrierAccountID;
                index++; row[index] = group.OurZone == null ? DBNull.Value : (object)group.OurZone.ZoneID;
                index++; row[index] = group.OriginatingZone == null ? DBNull.Value : (object)group.OriginatingZone.ZoneID;
                index++; row[index] = group.Supplier == null ? DBNull.Value : (object)group.Supplier.CarrierAccountID;
                index++; row[index] = group.SupplierZone == null ? DBNull.Value : (object)group.SupplierZone.ZoneID;
                index++; row[index] = group.Attempts;
                index++; row[index] = group.DeliveredAttempts;
                index++; row[index] = group.SuccessfulAttempts;
                index++; row[index] = group.DurationsInSeconds;
                index++; row[index] = group.PDDInSeconds.HasValue ? (object)group.PDDInSeconds : DBNull.Value;
                index++; row[index] = group.UtilizationInSeconds;
                index++; row[index] = group.NumberOfCalls;
                index++; row[index] = group.DeliveredNumberOfCalls;
                index++; row[index] = group.PGAD;
                index++; row[index] = group.CeiledDuration;
                index++; row[index] = group.MaxDurationInSeconds;
                index++; row[index] = group.ReleaseSourceAParty;
                DailystatsTable.Rows.Add(row);
            }
            DailystatsTable.EndLoadData();
            return DailystatsTable;
        }

        public void WriteDailyTrafficStat(IEnumerable<TrafficStats> stats)
        {
            lock (typeof(TrafficStats))
            {
                using (SqlTransaction transaction = this.Connection.BeginTransaction())
                {
                    try
                    {
                        DataTable _Stats = GetDailyTrafficStatsTable(stats, "TrafficStatsDaily");
                        WriteTrafficStats(_Stats, transaction, SqlBulkCopyOptions.KeepNulls);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        try { transaction.Rollback(); }
                        catch { }
                        log.Error("Error Performing Bulk Operation for Traffic Stats", ex);
                        throw (ex);
                    }
                }
            }
        }



        protected static DataTable OriginatingTrafficStatsDailyTable
        {
            get
            {
                lock (typeof(BulkManager))
                {
                    if (_TrafficStatsByOriginationDaily == null)
                    {
                        _TrafficStatsByOriginationDaily = new DataTable();
                        _TrafficStatsByOriginationDaily.Columns.Add("ID", typeof(long));
                        _TrafficStatsByOriginationDaily.Columns.Add("CallDate", typeof(DateTime));
                        _TrafficStatsByOriginationDaily.Columns.Add("SwitchID", typeof(int));
                        _TrafficStatsByOriginationDaily.Columns.Add("CustomerID", typeof(string));
                        _TrafficStatsByOriginationDaily.Columns.Add("OriginatingZoneID", typeof(int));
                        _TrafficStatsByOriginationDaily.Columns.Add("SupplierID", typeof(string));
                        _TrafficStatsByOriginationDaily.Columns.Add("SupplierZoneID", typeof(int));
                        _TrafficStatsByOriginationDaily.Columns.Add("Attempts", typeof(int));
                        _TrafficStatsByOriginationDaily.Columns.Add("DeliveredAttempts", typeof(int));
                        _TrafficStatsByOriginationDaily.Columns.Add("SuccessfulAttempts", typeof(int));
                        _TrafficStatsByOriginationDaily.Columns.Add("DurationsInSeconds", typeof(decimal));
                        _TrafficStatsByOriginationDaily.Columns.Add("PDDInSeconds", typeof(decimal));
                        _TrafficStatsByOriginationDaily.Columns.Add("UtilizationInSeconds", typeof(decimal));
                        _TrafficStatsByOriginationDaily.Columns.Add("NumberOfCalls", typeof(int));
                        _TrafficStatsByOriginationDaily.Columns.Add("DeliveredNumberOfCalls", typeof(int));
                        _TrafficStatsByOriginationDaily.Columns.Add("PGAD", typeof(int));
                        _TrafficStatsByOriginationDaily.Columns.Add("CeiledDuration", typeof(int));
                    }
                }
                return _TrafficStatsByOriginationDaily;
            }
        }


        protected DataTable GetOriginatingDailyTrafficStatsTable(IEnumerable<TrafficStats> stats, string tableName)
        {
            log.DebugFormat("Bulk Building Data table: {0}", tableName);
            DataTable OriginatingDailystatsTable = OriginatingTrafficStatsDailyTable.Clone();
            OriginatingDailystatsTable.TableName = tableName;
            OriginatingDailystatsTable.BeginLoadData();
            object MaxIDO = TABS.DataHelper.ExecuteScalar("SELECT MAX(ID) AS MaxID FROM TrafficStatsByOriginationDaily tsd", null);
            long maxID = 0;
            if (MaxIDO.ToString() != string.Empty)
                maxID = (long)MaxIDO;
            maxID += 1;
            foreach (var group in stats)
            {
                DataRow row = OriginatingDailystatsTable.NewRow();
                int index = -1;
                index++; row[index] = maxID; maxID += 1;
                index++; row[index] = group.CallDate;
                index++; row[index] = group.Switch.SwitchID;
                index++; row[index] = group.Customer == null ? DBNull.Value : (object)group.Customer.CarrierAccountID;
                index++; row[index] = group.OriginatingZone == null ? DBNull.Value : (object)group.OriginatingZone.ZoneID;
                index++; row[index] = group.Supplier == null ? DBNull.Value : (object)group.Supplier.CarrierAccountID;
                index++; row[index] = group.SupplierZone == null ? DBNull.Value : (object)group.SupplierZone.ZoneID;
                index++; row[index] = group.Attempts;
                index++; row[index] = group.DeliveredAttempts;
                index++; row[index] = group.SuccessfulAttempts;
                index++; row[index] = group.DurationsInSeconds;
                index++; row[index] = group.PDDInSeconds.HasValue ? (object)group.PDDInSeconds : DBNull.Value;
                index++; row[index] = group.UtilizationInSeconds;
                index++; row[index] = group.NumberOfCalls;
                index++; row[index] = group.DeliveredNumberOfCalls;
                index++; row[index] = group.PGAD;
                index++; row[index] = group.CeiledDuration;

                OriginatingDailystatsTable.Rows.Add(row);
            }
            OriginatingDailystatsTable.EndLoadData();
            return OriginatingDailystatsTable;
        }

        public void WriteOriginatingDailyTrafficStat(IEnumerable<TrafficStats> stats)
        {
            lock (typeof(TrafficStats))
            {
                SqlTransaction transaction = this.Connection.BeginTransaction();
                try
                {
                    DataTable _Stats = GetOriginatingDailyTrafficStatsTable(stats, "TrafficStatsByOriginationDaily");
                    WriteTrafficStats(_Stats, transaction, SqlBulkCopyOptions.KeepNulls);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    try { transaction.Rollback(); }
                    catch { }
                    log.Error("Error Performing Bulk Operation for Traffic Stats", ex);
                    throw (ex);
                }
            }
        }

        /// <summary>
        /// Write a data table to the database using the specified transaction and bulk copy options
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="transaction"></param>
        /// <param name="options"></param>
        protected void Write(DataTable dataTable, SqlTransaction transaction, SqlBulkCopyOptions options)
        {
            Write(dataTable, transaction, options, dataTable.TableName);
        }


        /// <summary>
        /// Write a data table to the database using the specified transaction and bulk copy options
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="transaction"></param>
        /// <param name="options"></param>
        public void Write(DataTable dataTable, SqlTransaction transaction, SqlBulkCopyOptions options, string DestinationTableName)
        {
            log.DebugFormat("Bulk Writing To {0}, {1} Records", DestinationTableName, dataTable.Rows.Count);
            try
            {
                if (dataTable.Rows.Count > 0)
                {
                    SqlBulkCopy bulkCopy = new SqlBulkCopy(this.Connection, options, transaction);
                    bulkCopy.DestinationTableName = DestinationTableName;
                    foreach (DataColumn column in dataTable.Columns)
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    bulkCopy.BulkCopyTimeout = 10 * 60; // 10 minutes
                    bulkCopy.WriteToServer(dataTable);
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error Bulk Writing To {0}", DestinationTableName), ex);
                throw (ex);
            }
        }


        /// <summary>
        /// Write a data table to the database using the specified transaction and bulk copy options
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="transaction"></param>
        /// <param name="options"></param>
        protected void WriteTrafficStats(DataTable dataTable, SqlTransaction transaction, SqlBulkCopyOptions options)
        {
            log.DebugFormat("Bulk Writing {0}, {1} Records", dataTable.TableName, dataTable.Rows.Count);
            try
            {
                if (dataTable.Rows.Count > 0)
                {
                    SqlBulkCopy bulkCopy = new SqlBulkCopy(this.Connection, options, transaction);
                    bulkCopy.DestinationTableName = dataTable.TableName;
                    //foreach (DataColumn column in dataTable.Columns)
                    //    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    bulkCopy.BulkCopyTimeout = 10 * 60; // 10 minutes
                    bulkCopy.WriteToServer(dataTable);
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error Bulk Writing {0}", dataTable.TableName), ex);
                throw (ex);
            }
        }


        /// <summary>
        /// Write the specified billing stats into the database
        /// </summary>
        /// <param name="billingStats"></param>
        public void Write(IEnumerable<Billing_Stats> billingStats)
        {
            using (SqlTransaction transaction = this.Connection.BeginTransaction())
            {
                try
                {
                    DataTable _Stats = GetBillingStatsTable(billingStats, "Billing_Stats");
                    Write(_Stats, transaction, SqlBulkCopyOptions.KeepNulls);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    try { transaction.Rollback(); }
                    catch { }
                    log.Error("Error Performing Bulk Operation for Billing Stats", ex);
                    throw (ex);
                }
            }
        }
        public void Write(List<TABS.Code> codes)
        {
            using (SqlTransaction transaction = this.Connection.BeginTransaction())
            {
                try
                {
                    DataTable _Codes = GetCodeTable(codes, "Code");
                    Write(_Codes, transaction, SqlBulkCopyOptions.KeepNulls);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    try { transaction.Rollback(); }
                    catch { }
                    log.Error("Error Performing Bulk Operation for Code table", ex);
                    throw (ex);
                }
            }
        }
        /// <summary>
        /// Write the specified billing stats into the database
        /// </summary>
        /// <param name="billingStats"></param>
        public void Write(IEnumerable<CDR> cdrs)
        {
            using (SqlTransaction transaction = this.Connection.BeginTransaction())
            {
                try
                {
                    DataTable cdrTable = GetCdrTable(cdrs, "CDR");
                    Write(cdrTable, transaction, SqlBulkCopyOptions.KeepNulls);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    try { transaction.Rollback(); }
                    catch { }
                    log.Error("Error Performing Bulk Operation for CDR", ex);
                    throw (ex);
                }
            }
        }

        /// <summary>
        /// Store the billing Cdrs and all pricing information (when available)
        /// In repricing mode the IDs will be negative
        /// </summary>
        /// <param name="cdrs"></param>
        public long Write(IEnumerable<Billing_CDR_Base> cdrs, bool repricingMode)
        {
            long lastpricedMainID = -1;
            using (SqlTransaction transaction = this.Connection.BeginTransaction())
            {
                try
                {
                    lock (typeof(Billing_CDR_Main))
                    {
                        // Get the Inserting ID 
                        string sql = repricingMode ? " SELECT ISNULL(MIN(ID),0) * 1 FROM Billing_CDR_Main " : " SELECT ISNULL(MAX(ID),0) * 1 FROM Billing_CDR_Main ";
                        lastpricedMainID = long.Parse(SecondaryDataHelper.ExecuteScalar(sql).ToString());

                        // If Repricing, should not be more than 0
                        if (repricingMode)
                        {
                            if (lastpricedMainID > 0) lastpricedMainID = 0;
                        }
                        else // If not prepricing, should not be less than last priced CDr param
                        {
                            var param = SystemConfiguration.KnownParameters[KnownSystemParameter.sys_CDR_Pricing_CDRID];
                            if (lastpricedMainID < param.NumericValue.Value)
                                lastpricedMainID = (long)param.NumericValue.Value;
                        }

                        // Assign IDs
                        foreach (var cdr in cdrs) if (cdr.IsValid) cdr.ID = repricingMode ? --lastpricedMainID : ++lastpricedMainID;

                        // Main 
                        DataTable _Main = GetBillingCdrTable(cdrs.Where(c => c.IsValid), "Billing_CDR_Main");
                        Write(_Main, transaction, SqlBulkCopyOptions.KeepIdentity);
                    }

                    // Cost and Sale
                    WriteBillingSaleAndCost(cdrs, transaction);

                    lock (typeof(Billing_CDR_Invalid))
                    {
                        // Get the Invalid Inserting ID
                        long invalidId = (long)(SystemParameter.Billing_CDR_Invalid_MaxId.NumericValue);

                        foreach (var cdr in cdrs) if (!cdr.IsValid) cdr.ID = ++invalidId;

                        // Invalid
                        DataTable invalidCdrsData = GetBillingCdrTable(cdrs.Where(c => !c.IsValid), "Billing_CDR_Invalid");
                        Write(invalidCdrsData, transaction, SqlBulkCopyOptions.KeepIdentity);

                        SystemParameter.Billing_CDR_Invalid_MaxId.NumericValue = invalidId;
                        DataHelper.ExecuteNonQuery("UPDATE SystemParameter SET NumericValue = @P1 WHERE Name = @P2", invalidId, SystemParameter.Billing_CDR_Invalid_MaxId.Name);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error(string.Format("Error Performing Bulk Operation"), ex);
                    throw (ex);
                }
                finally
                {

                }

                return lastpricedMainID;
            }
        }

        /// <summary>
        /// Write Sale and Cost records for the specified CDRs using transaction
        /// </summary>
        /// <param name="cdrs"></param>
        /// <param name="transaction"></param>
        private void WriteBillingSaleAndCost(IEnumerable<Billing_CDR_Base> cdrs, SqlTransaction transaction)
        {
            // Cost
            DataTable _Cost = GetPricingTable(cdrs.Where(c => c.IsValid && ((Billing_CDR_Main)c).Billing_CDR_Cost != null).Select(c => (Billing_CDR_Pricing_Base)((Billing_CDR_Main)c).Billing_CDR_Cost), "Billing_CDR_Cost");
            Write(_Cost, transaction, SqlBulkCopyOptions.Default);

            // Sale
            DataTable _Sale = GetPricingTable(cdrs.Where(c => c.IsValid && ((Billing_CDR_Main)c).Billing_CDR_Sale != null).Select(c => (Billing_CDR_Pricing_Base)((Billing_CDR_Main)c).Billing_CDR_Sale), "Billing_CDR_Sale");
            Write(_Sale, transaction, SqlBulkCopyOptions.Default);
        }

        /// <summary>
        /// Write Sale and Cost records for the specified CDRs 
        /// </summary>
        /// <param name="cdrs"></param>
        public void WriteBillingSaleAndCost(IEnumerable<Billing_CDR_Base> cdrs)
        {
            using (SqlTransaction transaction = this.Connection.BeginTransaction())
            {
                try
                {
                    WriteBillingSaleAndCost(cdrs, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    try { transaction.Rollback(); }
                    catch { }
                    throw ex;
                }
            }
        }

        protected DataTable GetPricingTable(IEnumerable<Billing_CDR_Pricing_Base> pricingData, string tableName)
        {
            log.DebugFormat("Bulk Building Data table: {0}", tableName);
            DataTable pricingTable = PricingTable.Clone();
            pricingTable.TableName = tableName;
            pricingTable.BeginLoadData();
            foreach (var pricing in pricingData)
            {
                DataRow row = pricingTable.NewRow();
                int index = -1;
                index++; row[index] = pricing.Billing_CDR_Main.ID;
                index++; row[index] = pricing.Zone.ZoneID;
                index++; row[index] = pricing.Net;
                index++; row[index] = pricing.Currency.Symbol;
                index++; row[index] = pricing.RateValue;
                index++; row[index] = pricing.Rate.ID;
                index++; row[index] = pricing.Discount.HasValue ? (object)pricing.Discount.Value : DBNull.Value;
                index++; row[index] = pricing.RateType;
                index++; row[index] = pricing.ToDConsideration == null ? DBNull.Value : (object)pricing.ToDConsideration.ToDConsiderationID;
                index++; row[index] = pricing.FirstPeriod.HasValue ? (object)pricing.FirstPeriod.Value : DBNull.Value;
                index++; row[index] = pricing.RepeatFirstperiod.HasValue ? (object)pricing.RepeatFirstperiod.Value : DBNull.Value;
                index++; row[index] = pricing.FractionUnit.HasValue ? (object)pricing.FractionUnit.Value : DBNull.Value;
                index++; row[index] = pricing.Tariff == null ? DBNull.Value : (object)pricing.Tariff.TariffID;
                index++; row[index] = pricing.CommissionValue;
                index++; row[index] = pricing.Commission == null ? DBNull.Value : (object)pricing.Commission.CommissionID;
                index++; row[index] = pricing.ExtraChargeValue;
                index++; row[index] = pricing.ExtraCharge == null ? DBNull.Value : (object)pricing.ExtraCharge.CommissionID;
                index++; row[index] = pricing.Updated;
                index++; row[index] = pricing.DurationInSeconds;
                index++; row[index] = pricing.Code == null ? DBNull.Value : (object)pricing.Code;
                index++; row[index] = pricing.Attempt = pricing.Billing_CDR_Main.Attempt;
                pricingTable.Rows.Add(row);
            }
            pricingTable.EndLoadData();
            return pricingTable;
        }

        protected DataTable GetBillingCdrTable(IEnumerable<Billing_CDR_Base> cdrData, string tableName)
        {
            log.DebugFormat("Bulk Building Data table: {0}", tableName);
            DataTable cdrTable = BillingCdrTable.Clone();
            bool includeRerouted = tableName.ToLower().EndsWith("invalid");
            if (!includeRerouted) cdrTable.Columns.Remove("IsRerouted");
            cdrTable.TableName = tableName;
            cdrTable.BeginLoadData();
            foreach (var cdr in cdrData)
            {
                DataRow row = cdrTable.NewRow();
                int index = -1;
                index++; row[index] = cdr.ID;
                index++; row[index] = cdr.Attempt;
                index++; row[index] = cdr.Alert.HasValue ? (object)cdr.Alert : DBNull.Value;
                index++; row[index] = cdr.Connect.HasValue ? (object)cdr.Connect : DBNull.Value;
                index++; row[index] = cdr.Disconnect.HasValue ? (object)cdr.Disconnect : DBNull.Value;
                index++; row[index] = cdr.DurationInSeconds;
                index++; row[index] = cdr.CustomerID;
                index++; row[index] = cdr.OurZone == null ? DBNull.Value : (object)cdr.OurZone.ZoneID;
                index++; row[index] = cdr.OriginatingZone == null ? DBNull.Value : (object)cdr.OriginatingZone.ZoneID;
                index++; row[index] = cdr.SupplierID;
                index++; row[index] = cdr.SupplierZone == null ? DBNull.Value : (object)cdr.SupplierZone.ZoneID;
                index++; row[index] = cdr.CDPN;
                index++; row[index] = cdr.CGPN;
                index++; row[index] = cdr.CDPNOut;
                index++; row[index] = cdr.ReleaseCode;
                index++; row[index] = cdr.ReleaseSource;
                index++; row[index] = cdr.Switch.SwitchID;
                index++; row[index] = cdr.SwitchCdrID;
                index++; row[index] = cdr.Tag;
                index++; row[index] = cdr.Extra_Fields;
                index++; row[index] = cdr.Port_IN;
                index++; row[index] = cdr.Port_OUT;
                index++; row[index] = cdr.OurCode != null ? (object)cdr.OurCode : DBNull.Value;
                index++; row[index] = cdr.SupplierCode != null ? (object)cdr.SupplierCode : DBNull.Value;

                if (includeRerouted) { index++; row[index] = cdr.IsRerouted ? "Y" : "N"; }
                cdrTable.Rows.Add(row);
            }
            cdrTable.EndLoadData();
            return cdrTable;
        }

        protected DataTable GetTrafficStatsTable(IEnumerable<TrafficStats> stats, string tableName)
        {
            log.DebugFormat("Bulk Building Data table: {0}", tableName);
            DataTable statsTable = TrafficStatsTable.Clone();
            statsTable.TableName = tableName;
            statsTable.BeginLoadData();
            foreach (var group in stats)
            {
                DataRow row = statsTable.NewRow();
                int index = -1;
                index++; row[index] = group.Switch.SwitchID;
                index++; row[index] = group.Port_IN;
                index++; row[index] = group.Port_OUT;
                index++; row[index] = group.Customer == null ? DBNull.Value : (object)group.Customer.CarrierAccountID;
                index++; row[index] = group.OurZone == null ? DBNull.Value : (object)group.OurZone.ZoneID;
                index++; row[index] = group.OriginatingZone == null ? DBNull.Value : (object)group.OriginatingZone.ZoneID;
                index++; row[index] = group.Supplier == null ? DBNull.Value : (object)group.Supplier.CarrierAccountID;
                index++; row[index] = group.SupplierZone == null ? DBNull.Value : (object)group.SupplierZone.ZoneID;
                index++; row[index] = group.FirstCDRAttempt;
                index++; row[index] = group.LastCDRAttempt;
                index++; row[index] = group.Attempts;
                index++; row[index] = group.DeliveredAttempts;
                index++; row[index] = group.SuccessfulAttempts;
                index++; row[index] = group.DurationsInSeconds;
                index++; row[index] = group.PDDInSeconds.HasValue ? (object)group.PDDInSeconds : DBNull.Value;
                index++; row[index] = group.MaxDurationInSeconds;
                index++; row[index] = group.UtilizationInSeconds;
                index++; row[index] = group.NumberOfCalls;
                index++; row[index] = group.DeliveredNumberOfCalls;
                index++; row[index] = group.PGAD;
                index++; row[index] = group.CeiledDuration;
                index++; row[index] = group.ReleaseSourceAParty;
                statsTable.Rows.Add(row);
            }
            statsTable.EndLoadData();
            return statsTable;
        }

        protected DataTable GetTrafficStatsTempTable(IEnumerable<TrafficStats> stats, string tableName)
        {
            log.DebugFormat("Bulk Building Data table: {0}", tableName);
            DataTable statsTable = TrafficStatsTempTable.Clone();
            statsTable.TableName = tableName;
            statsTable.BeginLoadData();
            foreach (var group in stats)
            {
                DataRow row = statsTable.NewRow();
                int index = -1;
                index++; row[index] = group.ID;
                index++; row[index] = group.Switch.SwitchID;
                index++; row[index] = group.Port_IN;
                index++; row[index] = group.Port_OUT;
                index++; row[index] = group.Customer == null ? DBNull.Value : (object)group.Customer.CarrierAccountID;
                index++; row[index] = group.OurZone == null ? DBNull.Value : (object)group.OurZone.ZoneID;
                index++; row[index] = group.OriginatingZone == null ? DBNull.Value : (object)group.OriginatingZone.ZoneID;
                index++; row[index] = group.Supplier == null ? DBNull.Value : (object)group.Supplier.CarrierAccountID;
                index++; row[index] = group.SupplierZone == null ? DBNull.Value : (object)group.SupplierZone.ZoneID;
                index++; row[index] = group.FirstCDRAttempt;
                index++; row[index] = group.LastCDRAttempt;
                index++; row[index] = group.Attempts;
                index++; row[index] = group.DeliveredAttempts;
                index++; row[index] = group.SuccessfulAttempts;
                index++; row[index] = group.DurationsInSeconds;
                index++; row[index] = group.PDDInSeconds.HasValue ? (object)group.PDDInSeconds : DBNull.Value;
                index++; row[index] = group.MaxDurationInSeconds;
                index++; row[index] = group.UtilizationInSeconds;
                index++; row[index] = group.NumberOfCalls;
                index++; row[index] = group.DeliveredNumberOfCalls;
                index++; row[index] = group.PGAD;
                index++; row[index] = group.CeiledDuration;
                index++; row[index] = group.ReleaseSourceAParty;
                statsTable.Rows.Add(row);
            }
            statsTable.EndLoadData();
            return statsTable;
        }

        protected DataTable GetBillingStatsTable(IEnumerable<Billing_Stats> stats, string tableName)
        {
            log.DebugFormat("Bulk Building Data table: {0}", tableName);
            DataTable statsTable = BillingStatsTable.Clone();
            statsTable.TableName = tableName;
            statsTable.BeginLoadData();
            foreach (var group in stats)
            {
                DataRow row = statsTable.NewRow();
                int index = -1;
                index++; row[index] = group.CallDate;
                index++; row[index] = group.Customer.CarrierAccountID;
                index++; row[index] = group.Supplier.CarrierAccountID;
                index++; row[index] = group.CostZone.ZoneID;
                index++; row[index] = group.SaleZone.ZoneID;
                index++; row[index] = group.Cost_Currency == null ? DBNull.Value : (object)group.Cost_Currency.Symbol;
                index++; row[index] = group.Sale_Currency == null ? DBNull.Value : (object)group.Sale_Currency.Symbol;
                index++; row[index] = group.SaleDuration;
                index++; row[index] = group.CostDuration;
                index++; row[index] = group.NumberOfCalls;
                index++; row[index] = group.FirstCallTime;
                index++; row[index] = group.LastCallTime;
                index++; row[index] = group.MinDuration;
                index++; row[index] = group.MaxDuration;
                index++; row[index] = group.AvgDuration;
                index++; row[index] = group.Cost_Nets;
                index++; row[index] = group.Cost_Discounts;
                index++; row[index] = group.Cost_Commissions;
                index++; row[index] = group.Cost_ExtraCharges;
                index++; row[index] = group.Sale_Nets;
                index++; row[index] = group.Sale_Discounts;
                index++; row[index] = group.Sale_Commissions;
                index++; row[index] = group.Sale_ExtraCharges;
                index++; row[index] = group.Sale_Rate;
                index++; row[index] = group.Cost_Rate;
                statsTable.Rows.Add(row);
            }
            statsTable.EndLoadData();
            return statsTable;
        }

        protected DataTable GetCdrTable(IEnumerable<CDR> cdrData, string tableName)
        {
            log.DebugFormat("Bulk Building Data table: {0}", tableName);
            DataTable cdrTable = CdrTable.Clone();
            cdrTable.TableName = tableName;
            cdrTable.BeginLoadData();
            foreach (var cdr in cdrData)
            {
                DataRow row = cdrTable.NewRow();
                int index = -1;
                index++; row[index] = cdr.Switch.SwitchID;
                index++; row[index] = cdr.IDonSwitch;
                index++; row[index] = cdr.Tag;
                index++; row[index] = cdr.AttemptDateTime;
                index++; row[index] = cdr.AlertDateTime.HasValue ? (object)cdr.AlertDateTime : DBNull.Value;
                index++; row[index] = cdr.ConnectDateTime.HasValue ? (object)cdr.ConnectDateTime : DBNull.Value; ;
                index++; row[index] = cdr.DisconnectDateTime.HasValue ? (object)cdr.DisconnectDateTime : DBNull.Value; ;
                index++; row[index] = cdr.DurationInSeconds;
                index++; row[index] = cdr.IN_TRUNK;
                index++; row[index] = cdr.IN_CIRCUIT;
                index++; row[index] = cdr.IN_CARRIER;
                index++; row[index] = cdr.IN_IP;
                index++; row[index] = cdr.OUT_TRUNK;
                index++; row[index] = cdr.OUT_CIRCUIT;
                index++; row[index] = cdr.OUT_CARRIER;
                index++; row[index] = cdr.OUT_IP;
                index++; row[index] = cdr.CGPN;
                index++; row[index] = cdr.CDPN;
                index++; row[index] = cdr.CDPNOut;
                index++; row[index] = cdr.CAUSE_FROM;
                index++; row[index] = cdr.CAUSE_TO;
                index++; row[index] = cdr.CAUSE_FROM_RELEASE_CODE;
                index++; row[index] = cdr.CAUSE_TO_RELEASE_CODE;
                index++; row[index] = cdr.Extra_Fields;
                index++; row[index] = cdr.IsRerouted ? "Y" : "N";
                cdrTable.Rows.Add(row);
            }
            cdrTable.EndLoadData();
            return cdrTable;
        }

        protected DataTable GetCodeTable(List<TABS.Code> codesData, string tableName)
        {
            log.DebugFormat("Bulk Building Data table: {0}", tableName);
            DataTable codeTable = CodeTable.Clone();
            codeTable.TableName = tableName;
            codeTable.BeginLoadData();
            foreach (var code in codesData)
            {
                DataRow row = codeTable.NewRow();
                int index = -1;
                index++; row[index] = code.Value;
                index++; row[index] = code.Zone.ZoneID;
                index++; row[index] = code.BeginEffectiveDate.Value;
                index++; row[index] = code.EndEffectiveDate.HasValue ? code.EndEffectiveDate.Value : (object)DBNull.Value;
                index++; row[index] = code.User.ID;
                codeTable.Rows.Add(row);
            }
            codeTable.EndLoadData();
            return codeTable;
        }


        public BulkManager(log4net.ILog log)
        {
            this.log = log;
            this.Connection = (System.Data.SqlClient.SqlConnection)SecondaryDataHelper.GetOpenConnection();
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Connection.Dispose();
            if (BillingCdrTable != null)
                BillingCdrTable.Dispose();
            if (PricingTable != null)
                PricingTable.Dispose();
            if (BillingStatsTable != null)
                BillingStatsTable.Dispose();
            if (TrafficStatsTable != null)
                TrafficStatsTable.Dispose();
            if (CdrTable != null)
                CdrTable.Dispose();
            if (CodeTable != null)
                CodeTable.Dispose();
            if (TrafficStatsDailyTable != null)
                TrafficStatsDailyTable.Dispose();
            if (TrafficStatsTable != null)
                TrafficStatsTable.Dispose();
            if (TrafficStatsTempTable != null)
                TrafficStatsTempTable.Dispose();

            //if (_Zone != null)
            //    _Zone.Dispose();
            _BillingCdrTable = null; _PricingTable = null;
            _BillingStatsTable = null; _TrafficStatsTable = null;
            _CdrTable = null; _CodeTable = null; //_Zone = null;

        }
        public static void Clear()
        {
            if (BillingCdrTable != null)
                BillingCdrTable.Dispose();
            if (PricingTable != null)
                PricingTable.Dispose();
            if (BillingStatsTable != null)
                BillingStatsTable.Dispose();
            if (TrafficStatsTable != null)
                TrafficStatsTable.Dispose();
            if (CdrTable != null)
                CdrTable.Dispose();
            if (CodeTable != null)
                CodeTable.Dispose();
            //if (_Zone != null)
            //    _Zone.Dispose();
            _BillingCdrTable = null; _PricingTable = null;
            _BillingStatsTable = null; _TrafficStatsTable = null;
            _CdrTable = null; _CodeTable = null; //_Zone = null;

        }

        #endregion
    }
}
