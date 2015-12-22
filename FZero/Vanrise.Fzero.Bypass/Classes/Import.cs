using System;
using System.Data;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class Import
    {
        public static Import Save(Import Import)
        {
            Import CurrentImport = new Import();
            try
            {
                using (Entities context = new Entities())
                {
                    context.Imports.Add(Import);
                    context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Import.Save(" + Import.ID.ToString() + ")", err);
            }
            return CurrentImport;
        }

        public static void ImportCDRs(string filePath, string SourceName) // Import Calls
        {
            try
            {
                DataTable dt = null;
                if (filePath.Contains(".xls"))
                {
                    dt = GeneratedCall.GetDataFromExcel(filePath, SourceName);
                }

                else if (filePath.Contains(".xlsx"))
                {
                    dt = GeneratedCall.GetDataFromExcel(filePath, SourceName);
                }


                else if (filePath.Contains(".xml"))
                {
                    dt = GeneratedCall.GetDataFromXml(filePath, SourceName);
                }
                if (dt != null)
                {
                    GeneratedCall.Confirm(SourceName, dt, null);
                }
            }
            catch (Exception err)
            {
                throw err;
            }
            


        }

    }
}
