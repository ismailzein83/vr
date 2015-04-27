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


              string Number = numberProfile.Number;
              DateTime? Date_Day = numberProfile.Date_Day;
              int? Day_Hour = numberProfile.Day_Hour;
              int? Count_Out_Calls = numberProfile.Count_Out_Calls;
              int Count_In_Calls = numberProfile.Count_In_Calls;
              int? Count_Out_Fail = numberProfile.Count_Out_Fail;
              int Count_In_Fail = numberProfile.Count_In_Fail;
              decimal? Call_Out_Dur_Avg = numberProfile.Call_Out_Dur_Avg;
              decimal Call_In_Dur_Avg = numberProfile.Call_In_Dur_Avg;
              decimal? Total_Out_Volume = numberProfile.Total_Out_Volume;
              decimal Total_In_Volume = numberProfile.Total_In_Volume;
              int? Diff_Output_Numb = numberProfile.Diff_Output_Numb;
              int Diff_Input_Numbers = numberProfile.Diff_Input_Numbers;
              int? Diff_Dest_net = numberProfile.Diff_Dest_net;
              int? Diff_Sources_net = numberProfile.Diff_Sources_net;
              int? Total_BTS_In = numberProfile.Total_BTS_In;
              int? Total_BTS_Out = numberProfile.Total_BTS_Out;
              int? Count_Out_SMS = numberProfile.Count_Out_SMS;
              int? Count_In_SMS = numberProfile.Count_In_SMS;
              int? Total_IMEI = numberProfile.Total_IMEI;
              int? Count_Out_OnNet = numberProfile.Count_Out_OnNet;
              int? Count_In_OnNet = numberProfile.Count_In_OnNet;
              int? Count_Out_OffNet = numberProfile.Count_Out_OffNet;
              int? Count_In_OffNet = numberProfile.Count_In_OffNet;
              int? Total_BTS = numberProfile.Total_BTS;
              int? Count_Out_Inter = numberProfile.Count_Out_Inter;
              int? Count_In_Inter = numberProfile.Count_In_Inter;
              int? IsOnNet = numberProfile.IsOnNet;
              decimal? Total_Data_Volume = numberProfile.Total_Data_Volume;

          


            switch ((Criteria)criteria.CriteriaId)
            {

                        case Criteria.Ratio_Incoming_Calls_vs_Outgoing_Calls :
                                result = (decimal)(numberProfile.Count_In_Calls / numberProfile.Count_Out_Calls);
                                break; 


                        case Criteria.Count_of_Distinct_Destinations :

                                break; 


                        case Criteria.Count_outgoing_calls :

                                break;
 

                        case Criteria.Count_of_Total_BTS_Per_MSISDN :

                                break; 


                        case Criteria.Total_Originated_Volume :

                                break; 


                        case Criteria.Count_of_Total_IMEI_Per_MSISDN :

                                break; 


                        case Criteria.Ratio_Average_Incoming_Duration_vs_Average_Outgoing_Duration :

                                break; 


                        case Criteria.Ratio_OffNet_Originated_Calls_vs_OnNet_Originated_Calls :

                                break; 


                        case Criteria.Count_of_daily_active_hours :

                                break; 


                        case Criteria.Distinct_Destination_of_Night_Calls :

                                break; 


                        case Criteria.Voice_Only_Service_Usage :

                                break; 


                        case Criteria.Ratio_of_Distinct_Destination_vs_Total_Number_of_Calls :

                                break; 


                        case Criteria.Ratio_International_Originated_Vs_Outgoing_Calls :

                                break; 


                        case Criteria.Count_of_outgoing_during_peak_hours :

                                break;


                        case Criteria.Data_Usage:

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

    }
}
