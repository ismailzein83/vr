using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;


namespace Vanrise.Fzero.Bypass
{
    public partial class Dashboard
    {
        public static List<prSummary_Result> listprSummary_Result { get; set; }
        public static List<prGetSummaryClient_Result> listprGetSummaryClient_Result { get; set; }
        public static List<prVwOrigination_Result> listprVwOrigination_Result { get; set; }
        public static List<prVwCarrier_Result> listprVwCarrier_Result { get; set; }

        public static void GetDashboard(int ClientID, int MobileOperatorID, DateTime? FromAttemptDateTime, DateTime? ToAttemptDateTime, bool IsAdmin)
        {
            try
            {
                using (Entities context = new Entities())
                {
                    listprSummary_Result = context.prSummary(MobileOperatorID, FromAttemptDateTime, ToAttemptDateTime, ClientID).ToList();

                    listprGetSummaryClient_Result = context.prGetSummaryClient(MobileOperatorID, FromAttemptDateTime, ToAttemptDateTime, ClientID).ToList();

                    listprVwOrigination_Result = context.prVwOrigination(MobileOperatorID, FromAttemptDateTime, ToAttemptDateTime, ClientID, IsAdmin).ToList();

                    listprVwCarrier_Result = context.prVwCarrier(MobileOperatorID, FromAttemptDateTime, ToAttemptDateTime, ClientID, IsAdmin).ToList();
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
