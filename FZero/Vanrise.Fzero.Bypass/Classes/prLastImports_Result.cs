using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class prLastImports_Result
    {
        public static List<prLastImports_Result> GetViewLastImports()
           
        {
            try
            {
                using (Entities context = new Entities())
                {
                    return context.prLastImports().ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ViewLastImport.GetViewLastImports()", err);
            }

            return null;
        }
    }
}
