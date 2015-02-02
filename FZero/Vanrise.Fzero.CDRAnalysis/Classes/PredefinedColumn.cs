using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.CDRAnalysis
{
    public partial class PredefinedColumn
    {
        public static List<PredefinedColumn> GetPredefinedColumns()
           
        {
            List<PredefinedColumn> PredefinedColumnsList = new List<PredefinedColumn>();

            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    PredefinedColumnsList = context.PredefinedColumns
                                            .OrderByDescending(u => u.Name)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.PredefinedColumn.GetPredefinedColumns()", err);
            }

            return PredefinedColumnsList;
        }
    }
}
