using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class prSourceGenerates_Result
    {
        public static List<prSourceGenerates_Result> GetViewSourceGenerates(DateTime? FromAttemptDateTime, DateTime? ToAttemptDateTime)
           
        {

            try
            {
                using (Entities context = new Entities())
                {
                    return context.prSourceGenerates(FromAttemptDateTime, ToAttemptDateTime).ToList(); 
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ViewSourceGenerate.GetViewSourceGenerates()", err);
            }

            return null;
        }
    }
}
