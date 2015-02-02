using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class RecommendedAction
    {
        public static List<RecommendedAction> GetRecommendedActions()
           
        {
            List<RecommendedAction> RecommendedActionsList = new List<RecommendedAction>();

            try
            {
                using (Entities context = new Entities())
                {
                    RecommendedActionsList = context.RecommendedActions
                                            .OrderByDescending(u => u.Name)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.RecommendedAction.GetRecommendedActiones()", err);
            }

            return RecommendedActionsList;
        }
    }
}
