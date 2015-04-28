using System;
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
                    if (numberProfile.PeriodId ==(int) Period.Day)
                    {
                        if (numberProfile.Total_In_Volume != 0 && numberProfile.Total_Out_Volume != 0)
                            result = (decimal)(numberProfile.Count_In_Calls / numberProfile.Count_Out_Calls);
                        else
                            result = 0;
                    }
                            
                    else if (numberProfile.PeriodId == (int)Period.Hour)
                    {
                    }
                    break;


                        
                case Criteria.Count_of_Distinct_Destinations://2
                    if (numberProfile.PeriodId == (int)Period.Day)
                    {
                        result = (decimal)numberProfile.Diff_Output_Numb;
                    }
                    else if (numberProfile.PeriodId == (int)Period.Hour)
                    {
                        
                    }
                    break;


                case Criteria.Count_outgoing_calls://3
                    if (numberProfile.PeriodId == (int)Period.Day)
                    {
                        result = (decimal)numberProfile.Count_Out_Calls;
                    }
                    else if (numberProfile.PeriodId == (int)Period.Hour)
                    {
                    }
                    break;

                case Criteria.Count_of_Total_BTS_Per_MSISDN://4
                    if (numberProfile.PeriodId == (int)Period.Day)
                    {
                        if (numberProfile.Total_BTS != null)
                            result = (decimal)numberProfile.Total_BTS;
                        else
                            result = 0;
                    }
                    else if (numberProfile.PeriodId == (int)Period.Hour)
                    {
                    }
                    break;

                    
                case Criteria.Total_Originated_Volume://5
                    if (numberProfile.PeriodId == (int)Period.Day)
                    {
                        result = (decimal)numberProfile.Total_Out_Volume;
                    }
                    else if (numberProfile.PeriodId == (int)Period.Hour)
                    {
                    }
                    break;

                case Criteria.Count_of_Total_IMEI_Per_MSISDN://6
                    if (numberProfile.PeriodId == (int)Period.Day)
                    {
                        result = (decimal)numberProfile.Total_IMEI;
                    }
                    else if (numberProfile.PeriodId == (int)Period.Hour)
                    {
                    }
                    break;

                    
                case Criteria.Ratio_Average_Incoming_Duration_vs_Average_Outgoing_Duration://7
                    if (numberProfile.PeriodId == (int)Period.Day)
                    {
                        if (numberProfile.Call_In_Dur_Avg != 0 && numberProfile.Call_Out_Dur_Avg != 0)
                            result = (decimal)(numberProfile.Call_In_Dur_Avg / numberProfile.Call_Out_Dur_Avg);
                        else
                            result = 0;
                    }
                    else if (numberProfile.PeriodId == (int)Period.Hour)
                    {
                    }
                    break;

                    
                case Criteria.Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls://8
                    if (numberProfile.PeriodId == (int)Period.Day)
                    {
                        if (numberProfile.Count_In_OffNet != 0 && numberProfile.Count_Out_OffNet != 0)
                            result = (decimal)(numberProfile.Count_In_OffNet / numberProfile.Count_Out_OffNet);
                        else
                            result = 0;
                    }
                    else if (numberProfile.PeriodId == (int)Period.Hour)
                    {
                    }
                    break;

                    
                case Criteria.Count_of_daily_active_hours://9
                    if (numberProfile.PeriodId == (int)Period.Day)
                    {
                        // not needed
                    }
                    else if (numberProfile.PeriodId == (int)Period.Hour)
                    {
                    }
                    break;

                case Criteria.Distinct_Destination_of_Night_Calls://10
                    if (numberProfile.PeriodId == (int)Period.Day)
                    {
                        // not developed yet ( need to add field to DB that contains such aggregation )
                    }
                    else if (numberProfile.PeriodId == (int)Period.Hour)
                    {
                    }
                    break;
                

                case Criteria.Voice_Only_Service_Usage://11
                if (numberProfile.PeriodId == (int)Period.Day)
                {
                    if (numberProfile.Count_Out_SMS != null)
                        result = (decimal)numberProfile.Count_Out_SMS;
                    else
                        result = 0;
                }
                else if (numberProfile.PeriodId == (int)Period.Hour)
                {
                }
                break;


                case Criteria.Ratio_of_Distinct_Destination_vs_Total_Number_of_Calls://12
                if (numberProfile.PeriodId == (int)Period.Day)
                {
                    if (numberProfile.Diff_Output_Numb != 0 && numberProfile.Count_Out_Calls != 0)
                        result = (decimal)(numberProfile.Diff_Output_Numb / numberProfile.Count_Out_Calls);
                    else
                        result = 0;
                }
                else if (numberProfile.PeriodId == (int)Period.Hour)
                {
                }
                break;


                case Criteria.Ratio_International_Originated_Vs_Outgoing_Calls://13
                if (numberProfile.PeriodId == (int)Period.Day)
                {
                    if (numberProfile.Count_In_Inter != 0 && numberProfile.Count_Out_Calls != 0)
                        result = (decimal)(numberProfile.Count_In_Inter / numberProfile.Count_Out_Calls);
                    else
                        result = 0;
                }
                else if (numberProfile.PeriodId == (int)Period.Hour)
                {
                }
                break;


                case Criteria.Count_of_outgoing_during_peak_hours://14
                if (numberProfile.PeriodId == (int)Period.Day)
                {
                    // not needed
                }
                else if (numberProfile.PeriodId == (int)Period.Hour)
                {
                }
                break;
                
                case Criteria.Data_Usage://15
                if (numberProfile.PeriodId == (int)Period.Day)
                {
                    result = (decimal)(numberProfile.Total_Data_Volume);
                }
                else if (numberProfile.PeriodId == (int)Period.Hour)
                {
                }
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
