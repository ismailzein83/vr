using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class prSourceGeneratesRecieves_Result
    {
        public static List<prSourceGeneratesRecieves_Result> GetViewSourceGeneratesRecieves(DateTime? FromAttemptDateTime, DateTime? ToAttemptDateTime)
        {
            try
            {
                using (Entities context = new Entities())
                {
                    return context.prSourceGeneratesRecieves(FromAttemptDateTime, ToAttemptDateTime).ToList(); 
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ViewSourceGeneratesRecieve.GetViewSourceGeneratesRecieves()", err);
            }

            return null;
        }
    }
}
