using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.CDRAnalysis 
{
    public partial class StrategyPeriod
    {
        public static StrategyPeriod Load(int id)
        {
            StrategyPeriod strategyPeriod = new StrategyPeriod();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    strategyPeriod = context.StrategyPeriods
                        .Include(s => s.Strategy)
                        .Include(s => s.Criteria_Profile)
                        .Include(s => s.Period)
                        .Where(s => s.Id == id)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.StrategyPeriod.Load(" + id + ")", err);
            }
            return strategyPeriod;
        }

      

        public static List<StrategyPeriod> GetAll()
        {
            List<StrategyPeriod> strategyThresholds = new List<StrategyPeriod>();
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    strategyThresholds = context.StrategyPeriods
                           .Include(s => s.Strategy)
                           .Include(s => s.Criteria_Profile)
                           .Include(s => s.Period)
                        .OrderBy(s => s.StrategyId)
                       .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.StrategyPeriod.GetAll()", err);
            }
            return strategyThresholds;
        }
        //--------------------------------------------

        public static bool Save(StrategyPeriod strategyPeriod)
        {
            bool success = false;
            try
            {
                using (CallsNormalizationEntities context = new CallsNormalizationEntities())
                {
                    if (strategyPeriod.Id == 0)
                    {
                        context.StrategyPeriods.Add(strategyPeriod);
                    }
                    else
                    {
                        context.Entry(strategyPeriod).State = System.Data.EntityState.Modified;

                    }
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.strategyPeriod.Save(Id: " + strategyPeriod.Id + ")", err);
            }
            return success;
        }

        //--------------------------------------------










    }
}
