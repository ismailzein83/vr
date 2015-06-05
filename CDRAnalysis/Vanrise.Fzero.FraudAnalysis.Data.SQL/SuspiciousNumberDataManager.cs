﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class SuspiciousNumberDataManager : BaseSQLDataManager, ISuspiciousNumberDataManager
    {
        public SuspiciousNumberDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public void SaveSuspiciousNumbers(List<SuspiciousNumber> suspiciousNumbers)
        {

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (SuspiciousNumber suspiciousNumber in suspiciousNumbers)
            {
                List<string> sValues = new List<string>();

                for (int i = 1; i <= 18; i++)
                {
                    if (suspiciousNumber.CriteriaValues.Where(x => x.Key == i).Count() == 1)
                    {
                       sValues.Add( Math.Round(suspiciousNumber.CriteriaValues.Where(x => x.Key == i).FirstOrDefault().Value,2).ToString());
                    }
                    else
                    {
                        sValues.Add("");
                    }
                }



                stream.WriteRecord("0,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}",
                             new[] { suspiciousNumber.DateDay.Value.ToString(), suspiciousNumber.Number.ToString(), sValues[0], sValues[1], sValues[2], sValues[3], sValues[4], sValues[5], sValues[6], sValues[7], sValues[8], sValues[9], sValues[10], sValues[11], sValues[12], sValues[13], sValues[14], sValues[15], sValues[16], suspiciousNumber.SuspectionLevel.ToString(), suspiciousNumber.StrategyId.ToString(), null }
               );

            }

            stream.Close();

            InsertBulkToTable(
                new StreamBulkInsertInfo
                {
                    TableName = "[dbo].[SubscriberThresholds]",
                    Stream = stream,
                    TabLock = false,
                    KeepIdentity = false,
                    FieldSeparator = ','
                });
        }

        public void SaveNumberProfiles(List<NumberProfile> numberProfiles)
        {

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (NumberProfile numberProfile in numberProfiles)
            {

                stream.WriteRecord("0,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32}",
                             new[] 
                             
                                 { 
                                    numberProfile.SubscriberNumber.ToString(),	
                                    numberProfile.FromDate.ToString(),	
                                    numberProfile.ToDate.ToString()	,
                                    numberProfile.AggregateValues["CountOutCalls"].ToString()	,
                                    numberProfile.AggregateValues["DiffOutputNumb"].ToString()	,	
                                    numberProfile.AggregateValues["CountOutInters"].ToString()	,	
                                    numberProfile.AggregateValues["CountInInters"].ToString()	,
                                    "0",	
                                    "0",		
                                    "0",		
                                    numberProfile.AggregateValues["CallOutDurs"].ToString(),	
                                    "0",	
                                    "0",		
                                    numberProfile.AggregateValues["CountOutFails"].ToString()	,
                                    numberProfile.AggregateValues["CountInFails"].ToString()	,
                                    numberProfile.AggregateValues["TotalOutVolume"].ToString()	,
                                    numberProfile.AggregateValues["TotalInVolume"].ToString(),	
                                    numberProfile.AggregateValues["DiffInputNumbers"].ToString()	,
                                    numberProfile.AggregateValues["CountOutSMSs"].ToString()	,
                                    numberProfile.AggregateValues["TotalIMEI"].ToString()	,	
                                    numberProfile.AggregateValues["TotalBTS"].ToString()	,	
                                    numberProfile.IsOnNet.ToString()	,	
                                    numberProfile.AggregateValues["TotalDataVolume"].ToString()	,
                                    ((int)numberProfile.Period).ToString()		,
                                    numberProfile.AggregateValues["CountInCalls"].ToString()	,
                                    numberProfile.AggregateValues["CallInDurs"].ToString()	,	
                                    numberProfile.AggregateValues["CountOutOnNets"].ToString()	,	
                                    numberProfile.AggregateValues["CountInOnNets"].ToString()	,
                                    numberProfile.AggregateValues["CountOutOffNets"].ToString()	,
                                    numberProfile.AggregateValues["CountInOffNets"].ToString() ,
	                                numberProfile.AggregateValues["CountFailConsecutiveCalls"].ToString(),
	                                numberProfile.AggregateValues["CountConsecutiveCalls"].ToString(),
	                                numberProfile.AggregateValues["CountOutLowDurationCalls"].ToString()
                                 }
               );




            }

            stream.Close();

            InsertBulkToTable(
                new StreamBulkInsertInfo
                {
                    TableName = "[dbo].[NumberProfile]",
                    Stream = stream,
                    TabLock = false,
                    KeepIdentity = false,
                    FieldSeparator = ','
                });
        }

    }
}
