using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;

using Vanrise.Fzero.CDRAnalysis;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.CDRAnalysis
{
    public partial class Strategy_Min_Values 
    {
        //-------------------------------------

        public static Strategy_Min_Values Load(int id)
        {
            Strategy_Min_Values strategy_Min_Values = new Strategy_Min_Values();
            try
            {
                using (Entities context = new Entities())
                {
                    strategy_Min_Values = context.Strategy_Min_Values
                        .Include(s => s.Strategy)
                        .Include(s => s.Criteria_Profile)
                        .Where(s => s.Id == id)
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Min_Values.Load(" + id + ")", err);
            }
            return strategy_Min_Values;
        }
        //-------------------------------------

        public static List<Strategy_Min_Values> GetList(int strategyId, int criteriaId)
        {
            if (strategyId == 0 && criteriaId == 0)
                return GetAll();

            List<Strategy_Min_Values> strategy_Min_Values = new List<Strategy_Min_Values>();
            try
            {
                using (Entities context = new Entities())
                {

                    if (strategyId != 0 && criteriaId == 0)
                    {
                        strategy_Min_Values = context.Strategy_Min_Values
                            .Include(s => s.Strategy)
                            .Include(s => s.Criteria_Profile)
                            .Where(
                                s =>
                                (s.StrategyId == strategyId)

                            )
                            .ToList();
                    }

                    else if (strategyId == 0 && criteriaId != 0)
                    {
                        strategy_Min_Values = context.Strategy_Min_Values
                            .Include(s => s.Strategy)
                            .Include(s => s.Criteria_Profile)
                            .Where(
                                s =>
                                (s.CriteriaId == criteriaId)

                            )
                            .ToList();
                    }

                    else if (strategyId != 0 && criteriaId != 0)
                    {
                        strategy_Min_Values = context.Strategy_Min_Values
                            .Include(s => s.Strategy)
                            .Include(s => s.Criteria_Profile)
                            .Where(
                                s =>
                                (s.CriteriaId == criteriaId && s.StrategyId == strategyId)

                            )
                            .ToList();
                    }
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Min_Values.GetList()", err);
            }
            return strategy_Min_Values;


        }
        //-------------------------------------

        public static List<Strategy_Min_Values> GetAll()
        {
            List<Strategy_Min_Values> strategy_Min_Values = new List<Strategy_Min_Values>();
            try
            {
                using (Entities context = new Entities())
                {
                    strategy_Min_Values = context.Strategy_Min_Values
                        .Include(s => s.Strategy)
                        .Include(s => s.Criteria_Profile)
                       .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Min_Values.GetAll()", err);
            }
            return strategy_Min_Values;
        }

        //-------------------------------------
        public static bool Delete(int id)
        {
            Strategy_Min_Values strategy_Min_Values = new Strategy_Min_Values() { Id = id };
            return Delete(strategy_Min_Values);
        }
        //-------------------------------------
        private static bool Delete(Strategy_Min_Values strategy_Min_Values)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    context.Entry(strategy_Min_Values).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                    success = true;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Min_Values.Delete(Id: " + strategy_Min_Values.Id + ")", err);
            }
            return success;
        }
        //-------------------------------------

        public static bool Save(Strategy_Min_Values strategy_Min_Values)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {

                    if (strategy_Min_Values.Id == 0)
                    {
                        context.Strategy_Min_Values.Add(strategy_Min_Values);
                    }
                    else
                    {
                        context.Entry(strategy_Min_Values).State = System.Data.EntityState.Modified;

                    }
                    context.SaveChanges();

                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Min_Values.Save(Id: " + strategy_Min_Values.Id + ")", err);
            }
            return success;
        }
        //-------------------------------------
    }
}
