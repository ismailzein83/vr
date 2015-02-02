using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class prComparisons_Result
    {
        public static List<prComparisons_Result> GetViewComparisons(DateTime? FromCompareDateTime, DateTime? ToCompareDateTime)
           
        {
            try
            {
                using (Entities context = new Entities())
                {
                    return context.prComparisons(FromCompareDateTime, ToCompareDateTime).ToList(); 
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ViewComparison.GetViewComparisons()", err);
            }

            return null;
        }

    }
}
