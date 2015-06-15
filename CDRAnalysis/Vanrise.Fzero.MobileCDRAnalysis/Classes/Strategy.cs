using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;



namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class Strategy
    {
        public static Strategy Load(int id)
        {
            Strategy strategy = new Strategy();
            try
            {
                using (Entities context = new Entities())
                {

                    strategy = context.Strategies
                        .Where(s => s.Id == id)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.strategy.Load(" + id + ")", err);
            }
            return strategy;
        }

        
        public static List<Strategy> GetAll()
        {
            List<Strategy> strategies = new List<Strategy>();
            try
            {
                using (Entities context = new Entities())
                {
                    strategies = context.Strategies
                       .ToList(); 
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy.GetAll()", err);
            }
            return strategies;
        }
    }
}
