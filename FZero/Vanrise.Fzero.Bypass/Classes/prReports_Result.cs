using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class prReports_Result
    {
        public static List<prReports_Result> GetprReports_Results(DateTime? FromSentDateTime, DateTime? ToSentDateTime)
        {
            try
            {
                using (Entities context = new Entities())
                {
                    return context.prReports(FromSentDateTime, ToSentDateTime).ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ViewImport.GetViewImports()", err);
            }

            return null;
        }
    }
}
