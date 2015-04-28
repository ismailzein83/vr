using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NumberProfile
    {
        public string SubscriberNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int?  Count_Out_Calls { get; set; }
        public int?      Diff_Output_Numb { get; set; }
        public int?      Count_Out_Inter { get; set; }
        public int?      Count_In_Inter { get; set; }
        public decimal?  Call_Out_Dur_Avg { get; set; }
        public int ? Count_Out_Fail { get; set; }
        public int   Count_In_Fail { get; set; }
        public decimal?  Total_Out_Volume { get; set; }
        public decimal   Total_In_Volume { get; set; }
        public int       Diff_Input_Numbers { get; set; }
        public int?      Count_Out_SMS { get; set; }
        public int?      Total_IMEI { get; set; }
        public int?      Total_BTS { get; set; }
        public int?      IsOnNet { get; set; }
        public decimal?  Total_Data_Volume { get; set; }
        public int?  PeriodId { get; set; }                
        public int   Count_In_Calls { get; set; }
        public decimal   Call_In_Dur_Avg { get; set; }
        public int ?     Count_Out_OnNet { get; set; }
        public int?      Count_In_OnNet { get; set; }
        public int?      Count_Out_OffNet { get; set; }
        public int?      Count_In_OffNet { get; set; }
        

    }
}
