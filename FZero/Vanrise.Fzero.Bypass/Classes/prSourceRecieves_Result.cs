using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class prSourceRecieves_Result
    {
        public static List<prSourceRecieves_Result> GetViewSourceRecieves(DateTime? FromAttemptDateTime, DateTime? ToAttemptDateTime)
           
        {
            try
            {
                using (Entities context = new Entities())
                {
                    return context.prSourceRecieves(FromAttemptDateTime, ToAttemptDateTime).ToList(); 
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ViewSourceRecieve.GetViewSourceRecieves()", err);
            }

            return null;
        }
    }
}
