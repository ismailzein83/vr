using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class prCaseStatuses_Result
    {
        public static List<prCaseStatuses_Result> GetprCaseStatuses_Result(DateTime? FromAttemptDateTime, DateTime? ToAttemptDateTime)
           
        {
            try
            {
                using (Entities context = new Entities())
                {
                    return context.prCaseStatuses(FromAttemptDateTime, ToAttemptDateTime).ToList(); 
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.prCaseStatuses_Result.GetprCaseStatuses_Result()", err);
            }

            return null;
        }
    }
}
