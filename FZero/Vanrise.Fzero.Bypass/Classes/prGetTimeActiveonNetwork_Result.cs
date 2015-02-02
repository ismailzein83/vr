using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;


namespace Vanrise.Fzero.Bypass
{
    public partial class prGetTimeActiveonNetwork_Result
    {
        public static List<prGetTimeActiveonNetwork_Result> prGetTimeActiveonNetwork(int ClientID, int MobileOperatorID, DateTime? FromAttemptDateTime, DateTime? ToAttemptDateTime)
        {
            try
            {
                using (Entities context = new Entities())
                {

                    return context.prGetTimeActiveonNetwork (MobileOperatorID,FromAttemptDateTime, ToAttemptDateTime, ClientID).ToList();
                }
            }
            catch 
            {
                return null;
            }
        }
    }
}
