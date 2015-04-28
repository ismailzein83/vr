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
        public Decimal GetCriteriaValue(CriteriaDefinition criteria, NumberProfile numberProfile)
        {

            decimal result = 0;

            switch ((Criteria)criteria.CriteriaId)
            {
                case Criteria.Ratio_Incoming_Calls_vs_Outgoing_Calls ://1
                    if (numberProfile.totalInVolume != 0 && numberProfile.totalOutVolume != 0)
                        result = (decimal)(numberProfile.countInCalls / numberProfile.countOutCalls);
                    else
                        result = 0;
                    break;


                case Criteria.Count_of_Distinct_Destinations://2
                    result = (decimal)numberProfile.diffOutputNumb;
                    break;


                case Criteria.Count_outgoing_calls://3
                    result = (decimal)numberProfile.countOutCalls;
                    break;

                case Criteria.Count_of_Total_BTS_Per_MSISDN://4
                    if (numberProfile.totalBTS != null)
                        result = (decimal)numberProfile.totalBTS;
                    else
                        result = 0;
                    break;

                    
                case Criteria.Total_Originated_Volume://5
                    result = (decimal)numberProfile.totalOutVolume;
                    break;

                case Criteria.Count_of_Total_IMEI_Per_MSISDN://6
                    result = (decimal)numberProfile.totalIMEI;
                    break;

                    
                case Criteria.Ratio_Average_Incoming_Duration_vs_Average_Outgoing_Duration://7
                    if (numberProfile.callInDurAvg != 0 && numberProfile.callOutDurAvg != 0)
                        result = (decimal)(numberProfile.callInDurAvg / numberProfile.callOutDurAvg);
                    else
                        result = 0;
                    break;

                    
                case Criteria.Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls://8
                    if (numberProfile.countInOffNet != 0 && numberProfile.countOutOffNet != 0)
                        result = (decimal)(numberProfile.countInOffNet / numberProfile.countOutOffNet);
                    else
                        result = 0;
                    break;

                    
                case Criteria.Count_of_daily_active_hours://9
                    if (numberProfile.periodId == (int)Period.Day)
                    {
                        // not developed yet
                    }
                    break;

                case Criteria.Distinct_Destination_of_Night_Calls://10
                    if (numberProfile.periodId == (int)Period.Day)
                    {
                        // not developed yet
                    }
                    else if (numberProfile.periodId == (int)Period.Hour)
                    {
                        // not developed yet
                    }
                    break;
                

                case Criteria.Voice_Only_Service_Usage://11
                    if (numberProfile.countOutSMS != null)
                        result = (decimal)numberProfile.countOutSMS;
                    else
                        result = 0;
                break;


                case Criteria.Ratio_of_Distinct_Destination_vs_Total_Number_of_Calls://12
                    if (numberProfile.diffOutputNumb != 0 && numberProfile.countOutCalls != 0)
                        result = (decimal)(numberProfile.diffOutputNumb / numberProfile.countOutCalls);
                    else
                        result = 0;
                break;


                case Criteria.Ratio_International_Originated_Vs_Outgoing_Calls://13
                    if (numberProfile.countInInter != 0 && numberProfile.countOutCalls != 0)
                        result = (decimal)(numberProfile.countInInter / numberProfile.countOutCalls);
                    else
                        result = 0;
                break;


                case Criteria.Count_of_outgoing_during_peak_hours://14
                    if (numberProfile.periodId == (int)Period.Day)
                    {
                        // not developed yet
                    }
                break;
                
                case Criteria.Data_Usage://15
                    result = (decimal)(numberProfile.totalDataVolume);
                break;

            }

            return result;
        }

        public Dictionary<int, CriteriaDefinition> GetCriteriaDefinitions()
        {
            throw new NotImplementedException();
        }


        enum Criteria 
        { 
            Ratio_Incoming_Calls_vs_Outgoing_Calls = 1,
            Count_of_Distinct_Destinations = 2,
            Count_outgoing_calls = 3,
            Count_of_Total_BTS_Per_MSISDN = 4,
            Total_Originated_Volume = 5,
            Count_of_Total_IMEI_Per_MSISDN = 6,
            Ratio_Average_Incoming_Duration_vs_Average_Outgoing_Duration = 7,
            Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls = 8,
            Count_of_daily_active_hours = 9,
            Distinct_Destination_of_Night_Calls = 10,
            Voice_Only_Service_Usage = 11,
            Ratio_of_Distinct_Destination_vs_Total_Number_of_Calls = 12,
            Ratio_International_Originated_Vs_Outgoing_Calls = 13,
            Count_of_outgoing_during_peak_hours = 14,
            Data_Usage = 15
        };


        enum Period
        {
            Day = 6,
            Hour = 1
        };

    }
}
