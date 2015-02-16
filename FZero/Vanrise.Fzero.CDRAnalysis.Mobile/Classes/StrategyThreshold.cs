using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.CDRAnalysis.Mobile
{
    public partial class StrategyThreshold
    {

        public static StrategyThreshold Load(int id)
        {
            StrategyThreshold strategyThreshold = new StrategyThreshold();
            try
            {
                using (Entities context = new Entities())
                {
                    strategyThreshold = context.StrategyThresholds
                        .Include(s => s.Strategy)
                        .Include(s => s.Criteria_Profile)
                        .Where(s => s.Id == id)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.strategyThreshold.Load(" + id + ")", err);
            }
            return strategyThreshold;
        }
        //--------------------------------------------

        public static List<StrategyThreshold> GetList(int strategyId, int criteriaID)
        {
            if (strategyId == 0 && criteriaID == 0)
                return GetAll();

            List<StrategyThreshold> strategyThresholds = new List<StrategyThreshold>();
            try
            {
                using (Entities context = new Entities())
                {

                    if (strategyId != 0 && criteriaID == 0)
                    {
                        strategyThresholds = context.StrategyThresholds
                            .Include(s => s.Strategy)
                            .Include(s => s.Criteria_Profile)
                            .Where(
                                s =>
                                (s.StrategyId == strategyId)

                            )
                            .ToList();
                    }
                    else if (strategyId == 0 && criteriaID != 0)
                    {
                        strategyThresholds = context.StrategyThresholds
                            .Include(s => s.Strategy)
                            .Include(s => s.Criteria_Profile)
                            .Where(
                                s =>
                                (s.CriteriaID == criteriaID)

                            )
                            .ToList();
                    }
                    else if (strategyId != 0 && criteriaID != 0)
                    {
                        strategyThresholds = context.StrategyThresholds
                            .Include(s => s.Strategy)
                            .Include(s => s.Criteria_Profile)
                            .Where(
                                s =>
                                (s.CriteriaID == criteriaID && s.StrategyId == strategyId)

                            )
                            .ToList();
                    }




                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy.GetList()", err);
            }
            return strategyThresholds;
        }

        //--------------------------------------------

        public static List<StrategyThreshold> GetAll()
        {
            List<StrategyThreshold> strategyThresholds = new List<StrategyThreshold>();
            try
            {
                using (Entities context = new Entities())
                {
                    strategyThresholds = context.StrategyThresholds
                        .Include(s => s.Strategy.StrategyPeriods)
                        .Include(s => s.Criteria_Profile)
                        .OrderBy(s => s.StrategyId)
                       .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.StrategyThreshold.GetAll()", err);
            }
            return strategyThresholds;
        }

        //--------------------------------------------

        public static bool Delete(int id)
        {
            StrategyThreshold strategyThreshold = new StrategyThreshold() { Id = id };
            return Delete(strategyThreshold);
        }

        //--------------------------------------------

        private static bool Delete(StrategyThreshold strategyThreshold)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {

                    StrategyThreshold st = StrategyThreshold.Load(strategyThreshold.Id);
                    //if (st.NormalizationRules.Count() > 0 || st.SwitchTruncks.Count() > 0)
                    //{
                    //    success=false;
                    //}


                    context.Entry(st).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                    success = true;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.StrategyThreshold.Delete(Id: " + strategyThreshold.Id + ")", err);
            }
            return success;
        }

        //--------------------------------------------

        public static bool Save(StrategyThreshold strategyThreshold)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    if (strategyThreshold.Id == 0)
                    {
                        context.StrategyThresholds.Add(strategyThreshold);
                    }
                    else
                    {
                        context.Entry(strategyThreshold).State = System.Data.EntityState.Modified;

                    }
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.strategyThreshold.Save(Id: " + strategyThreshold.Id + ")", err);
            }
            return success;
        }

        //--------------------------------------------




    }
}
