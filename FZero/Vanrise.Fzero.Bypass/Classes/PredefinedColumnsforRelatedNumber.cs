using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class PredefinedColumnsforRelatedNumber
    {
        public static List<PredefinedColumnsforRelatedNumber> GetPredefinedColumnsforRelatedNumbers()
           
        {
            List<PredefinedColumnsforRelatedNumber> PredefinedColumnsforRelatedNumbersList = new List<PredefinedColumnsforRelatedNumber>();

            try
            {
                using (Entities context = new Entities())
                {
                    PredefinedColumnsforRelatedNumbersList = context.PredefinedColumnsforRelatedNumbers
                                            .OrderByDescending(u => u.Name)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.PredefinedColumnsforRelatedNumber.GetPredefinedColumnsforRelatedNumbers()", err);
            }

            return PredefinedColumnsforRelatedNumbersList;
        }
    }
}
