﻿using System;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class Import
    {
        public static Import Save(Import Import)
        {
            Import CurrentImport = new Import();
            try
            {
                using (MobileEntities context = new MobileEntities())
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
    }
}
