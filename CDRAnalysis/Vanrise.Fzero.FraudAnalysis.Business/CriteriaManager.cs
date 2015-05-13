﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class CriteriaManager
    {

        public Dictionary<int, decimal> GetCriteriaValues(NumberProfile numberProfile)
        {
            Dictionary<int, decimal> criteriaValues = new Dictionary<int,decimal>();

            foreach( var i in GetCriteriaDefinitions())
            {
                criteriaValues.Add(i.Key, GetCriteriaValue(new CriteriaDefinition(){ CriteriaId=i.Value.CriteriaId, CompareOperator=i.Value.CompareOperator,  Description=i.Value.Description }, numberProfile));
            }
            return criteriaValues;
        }

        public Decimal GetCriteriaValue(CriteriaDefinition criteria, NumberProfile numberProfile)
        {

            decimal? result = 0;

            decimal CountOutCalls = numberProfile.AggregateValues.Where(x => x.Key == "CountOutCalls").FirstOrDefault().Value;
            decimal DiffOutputNumb = numberProfile.AggregateValues.Where(x => x.Key == "DiffOutputNumb").FirstOrDefault().Value;
            decimal CountOutInter = numberProfile.AggregateValues.Where(x => x.Key == "CountOutInter").FirstOrDefault().Value;
            decimal CountInInter = numberProfile.AggregateValues.Where(x => x.Key == "CountInInter").FirstOrDefault().Value;
            decimal CallOutDurAvg = numberProfile.AggregateValues.Where(x => x.Key == "CallOutDurAvg").FirstOrDefault().Value;
            decimal CountOutFail = numberProfile.AggregateValues.Where(x => x.Key == "CountOutFail").FirstOrDefault().Value;
            decimal CountInFail = numberProfile.AggregateValues.Where(x => x.Key == "CountInFail").FirstOrDefault().Value;
            decimal TotalOutVolume = numberProfile.AggregateValues.Where(x => x.Key == "TotalOutVolume").FirstOrDefault().Value;
            decimal TotalInVolume = numberProfile.AggregateValues.Where(x => x.Key == "TotalInVolume").FirstOrDefault().Value;
            decimal DiffInputNumbers = numberProfile.AggregateValues.Where(x => x.Key == "DiffInputNumbers").FirstOrDefault().Value;
            decimal CountOutSMS = numberProfile.AggregateValues.Where(x => x.Key == "CountOutSMS").FirstOrDefault().Value;
            decimal TotalIMEI = numberProfile.AggregateValues.Where(x => x.Key == "TotalIMEI").FirstOrDefault().Value;
            decimal TotalBTS = numberProfile.AggregateValues.Where(x => x.Key == "TotalBTS").FirstOrDefault().Value;
            decimal CountInCalls = numberProfile.AggregateValues.Where(x => x.Key == "CountInCalls").FirstOrDefault().Value;
            decimal CallInDurAvg = numberProfile.AggregateValues.Where(x => x.Key == "CallInDurAvg").FirstOrDefault().Value;
            decimal CountOutOnNet = numberProfile.AggregateValues.Where(x => x.Key == "CountOutOnNet").FirstOrDefault().Value;
            decimal CountInOnNet = numberProfile.AggregateValues.Where(x => x.Key == "CountInOnNet").FirstOrDefault().Value;
            decimal CountOutOffNet = numberProfile.AggregateValues.Where(x => x.Key == "CountOutOffNet").FirstOrDefault().Value;
            decimal CountInOffNet = numberProfile.AggregateValues.Where(x => x.Key == "CountInOffNet").FirstOrDefault().Value;
            decimal TotalDataVolume = numberProfile.AggregateValues.Where(x => x.Key == "TotalDataVolume").FirstOrDefault().Value;


















            switch ((Enums.Criteria)criteria.CriteriaId)
            {
                case Enums.Criteria.Ratio_Incoming_Calls_vs_Outgoing_Calls://1
                    if (TotalInVolume != 0 && TotalOutVolume != 0)
                        result = (decimal?)(CountInCalls / CountOutCalls);
                    else
                        result = 0;
                    break;


                case Enums.Criteria.Count_of_Distinct_Destinations://2
                    result = (decimal?)DiffOutputNumb;
                    break;


                case Enums.Criteria.Count_outgoing_calls://3
                    result = (decimal?)CountOutCalls;
                    break;

                case Enums.Criteria.Count_of_Total_BTS_Per_MSISDN://4
                    if (TotalBTS != null)
                        result = (decimal?)TotalBTS;
                    else
                        result = 0;
                    break;


                case Enums.Criteria.Total_Originated_Volume://5
                    result = (decimal?)TotalOutVolume;
                    break;

                case Enums.Criteria.Count_of_Total_IMEI_Per_MSISDN://6
                    result = (decimal?)TotalIMEI;
                    break;


                case Enums.Criteria.Ratio_Average_Incoming_Duration_vs_Average_Outgoing_Duration://7
                    if (CallInDurAvg != 0 && CallOutDurAvg != 0)
                        result = (decimal?)(CallInDurAvg / CallOutDurAvg);
                    else
                        result = 0;
                    break;


                case Enums.Criteria.Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls://8
                    if (CountInOffNet != 0 && CountOutOffNet != 0)
                        result = (decimal?)(CountInOffNet / CountOutOffNet);
                    else
                        result = 0;
                    break;


                case Enums.Criteria.Count_of_daily_active_hours://9
                    if (numberProfile.PeriodId == (int)Enums.Period.Day)
                    {
                        // not developed yet
                    }
                    break;

                case Enums.Criteria.Distinct_Destination_of_Night_Calls://10
                    if (numberProfile.PeriodId == (int)Enums.Period.Day)
                    {
                        // not developed yet
                    }
                    else if (numberProfile.PeriodId == (int)Enums.Period.Hour)
                    {
                        // not developed yet
                    }
                    break;


                case Enums.Criteria.Voice_Only_Service_Usage://11
                    if (CountOutSMS != null)
                        result = (decimal?)CountOutSMS;
                    else
                        result = 0;
                break;


                case Enums.Criteria.Ratio_of_Distinct_Destination_vs_Total_Number_of_Calls://12
                    if (DiffOutputNumb != 0 && CountOutCalls != 0)
                        result = (decimal?)(DiffOutputNumb / CountOutCalls);
                    else
                        result = 0;
                break;


                case Enums.Criteria.Ratio_International_Originated_Vs_Outgoing_Calls://13
                    if (CountInInter != 0 && CountOutCalls != 0)
                        result = (decimal?)(CountInInter / CountOutCalls);
                    else
                        result = 0;
                break;


                case Enums.Criteria.Count_of_outgoing_during_peak_hours://14
                if (numberProfile.PeriodId == (int)Enums.Period.Day)
                    {
                        // not developed yet
                    }
                break;

                case Enums.Criteria.Data_Usage://15
                result = (decimal?)(TotalDataVolume);
                break;

            }

             if ( result==null)
                result=0;
            
            return result.Value;
        }

        public Dictionary<int, CriteriaDefinition> GetCriteriaDefinitions()
        {
            Dictionary<int, CriteriaDefinition> dictionary = new Dictionary<int, CriteriaDefinition>();

            dictionary.Add(1, new CriteriaDefinition() { CriteriaId = 1, Description = "Ratio_Incoming_Calls_vs_Outgoing_Calls", CompareOperator= CriteriaCompareOperator.LessThanorEqual });
            dictionary.Add(2, new CriteriaDefinition() { CriteriaId = 2, Description = "Count_of_Distinct_Destinations", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual });
            dictionary.Add(3, new CriteriaDefinition() { CriteriaId = 3, Description = "Count_outgoing_calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual });
            dictionary.Add(4, new CriteriaDefinition() { CriteriaId = 4, Description = "Count_of_Total_BTS_Per_MSISDN", CompareOperator = CriteriaCompareOperator.LessThanorEqual });
            dictionary.Add(5, new CriteriaDefinition() { CriteriaId = 5, Description = "Total_Originated_Volume", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual });
            dictionary.Add(6, new CriteriaDefinition() { CriteriaId = 6, Description = "Count_of_Total_IMEI_Per_MSISDN", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual });
            dictionary.Add(7, new CriteriaDefinition() { CriteriaId = 7, Description = "Ratio_Average_Incoming_Duration_vs_Average_Outgoing_Duration", CompareOperator = CriteriaCompareOperator.LessThanorEqual });
            dictionary.Add(8, new CriteriaDefinition() { CriteriaId = 8, Description = "Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual });
            dictionary.Add(9, new CriteriaDefinition() { CriteriaId = 9, Description = "Count_of_daily_active_hours", CompareOperator = CriteriaCompareOperator.LessThanorEqual });
            dictionary.Add(10, new CriteriaDefinition() { CriteriaId = 10, Description = "Distinct_Destination_of_Night_Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual });
            dictionary.Add(11, new CriteriaDefinition() { CriteriaId = 11, Description = "Voice_Only_Service_Usage", CompareOperator = CriteriaCompareOperator.LessThanorEqual });
            dictionary.Add(12, new CriteriaDefinition() { CriteriaId = 12, Description = "Ratio_of_Distinct_Destination_vs_Total_Number_of_Calls", CompareOperator = CriteriaCompareOperator.GreaterThanorEqual });
            dictionary.Add(13, new CriteriaDefinition() { CriteriaId = 13, Description = "Ratio_International_Originated_Vs_Outgoing_Calls", CompareOperator = CriteriaCompareOperator.LessThanorEqual });
            dictionary.Add(14, new CriteriaDefinition() { CriteriaId = 14, Description = "Count_of_outgoing_during_peak_hours", CompareOperator = CriteriaCompareOperator.LessThanorEqual });
            dictionary.Add(15, new CriteriaDefinition() { CriteriaId = 15, Description = "Data_Usage", CompareOperator = CriteriaCompareOperator.LessThanorEqual });

            return dictionary; 
        }


        //enum Criteria 
        //{ 
        //    Ratio_Incoming_Calls_vs_Outgoing_Calls = 1,
        //    Count_of_Distinct_Destinations = 2,
        //    Count_outgoing_calls = 3,
        //    Count_of_Total_BTS_Per_MSISDN = 4,
        //    Total_Originated_Volume = 5,
        //    Count_of_Total_IMEI_Per_MSISDN = 6,
        //    Ratio_Average_Incoming_Duration_vs_Average_Outgoing_Duration = 7,
        //    Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls = 8,
        //    Count_of_daily_active_hours = 9,
        //    Distinct_Destination_of_Night_Calls = 10,
        //    Voice_Only_Service_Usage = 11,
        //    Ratio_of_Distinct_Destination_vs_Total_Number_of_Calls = 12,
        //    Ratio_International_Originated_Vs_Outgoing_Calls = 13,
        //    Count_of_outgoing_during_peak_hours = 14,
        //    Data_Usage = 15
        //};


        //enum Period
        //{
        //    Day = 6,
        //    Hour = 1
        //};

    }
}
