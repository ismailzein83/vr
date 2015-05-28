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

                for (int i = 1; i <= 16; i++)
                {
                    if (suspiciousNumber.CriteriaValues.Where(x => x.Key == i).Count() == 1)
                    {
                        sValues.Add(Math.Round(suspiciousNumber.CriteriaValues.Where(x => x.Key == i).FirstOrDefault().Value).ToString());
                    }
                    else
                    {
                        sValues.Add("");
                    }
                }



                stream.WriteRecord("0,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}",
                             new[] { suspiciousNumber.DateDay.Value.ToString(), suspiciousNumber.Number.ToString(), sValues[0], sValues[1], sValues[2], sValues[3], sValues[4], sValues[5], sValues[6], sValues[7], sValues[8], sValues[9], sValues[10], sValues[11], sValues[12], sValues[13], sValues[14], sValues[15], suspiciousNumber.SuspectionLevel.ToString(), suspiciousNumber.StrategyId.ToString(), null }
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

                stream.WriteRecord("0,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29}",
                             new[] 
                             
                                 { 
                                    numberProfile.SubscriberNumber.ToString(),	
                                    numberProfile.FromDate.ToString(),	
                                    numberProfile.ToDate.ToString()	,
                                    Math.Round(numberProfile.AggregateValues["CountOutCalls"],0).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["DiffOutputNumb"],0).ToString()	,	
                                    Math.Round(numberProfile.AggregateValues["CountOutInters"],0).ToString()	,	
                                    Math.Round(numberProfile.AggregateValues["CountInInters"],0).ToString()	,
                                    "0",	
                                    "0",		
                                    "0",		
                                    numberProfile.AggregateValues["CallOutDurs"].ToString(),	
                                    "0",	
                                    "0",		
                                    Math.Round(numberProfile.AggregateValues["CountOutFails"],0).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["CountInFails"],0).ToString()	,
                                    numberProfile.AggregateValues["TotalOutVolume"].ToString()	,
                                    numberProfile.AggregateValues["TotalInVolume"].ToString(),	
                                    Math.Round(numberProfile.AggregateValues["DiffInputNumbers"],0).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["CountOutSMSs"],0).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["TotalIMEI"],0).ToString()	,	
                                    Math.Round(numberProfile.AggregateValues["TotalBTS"],0).ToString()	,	
                                    numberProfile.IsOnNet.ToString()	,	
                                    numberProfile.AggregateValues["TotalDataVolume"].ToString()	,
                                    ((int)numberProfile.Period).ToString()		,
                                    Math.Round(numberProfile.AggregateValues["CountInCalls"],0).ToString()	,
                                    numberProfile.AggregateValues["CallInDurs"].ToString()	,	
                                    Math.Round(numberProfile.AggregateValues["CountOutOnNets"],0).ToString()	,	
                                    Math.Round(numberProfile.AggregateValues["CountInOnNets"],0).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["CountOutOffNets"],0).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["CountInOffNets"],0).ToString()	
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
