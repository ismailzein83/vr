using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class prClientCases_Result
    {
        public static List<prClientCases_Result> prClientCases(DateTime? FromAttemptDateTime, DateTime? ToAttemptDateTime)
           
        {
            try
            {
                using (Entities context = new Entities())
                {
                    return context.prClientCases(FromAttemptDateTime, ToAttemptDateTime).ToList(); 
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ViewClientCase.GetViewClientCases()", err);
            }

            return null;
        }

    }
}
