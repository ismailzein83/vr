using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class prImports_Result
    {
        public static List<prImports_Result> GetViewImports(DateTime? FromImportDate, DateTime? ToImportDate)
           
        {
            try
            {
                using (Entities context = new Entities())
                {
                    return context.prImports(FromImportDate, ToImportDate).ToList(); 
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
