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
    public partial class Strategy_Suspection_Level
    {


       // public int SuspectionWeight { get; set; }



        public static Strategy_Suspection_Level Load(int id)
        {
            Strategy_Suspection_Level strategy_Suspection_level = new Strategy_Suspection_Level();
            try
            {
                using (Entities context = new Entities())
                {
                    strategy_Suspection_level = context.Strategy_Suspection_Level
                        .Include(s => s.Strategy)
                        .Where(s => s.Id == id)
                        .OrderBy(s => new { s.StrategyId, s.LevelId, s.CriteriaId1, s.CriteriaId2, s.CriteriaId3, s.CriteriaId4, s.CriteriaId5, s.CriteriaId6 })
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Suspection_level.Load(" + id + ")", err);
            }
            return strategy_Suspection_level;
        }
        //----------------------------------------
        public static List<Strategy_Suspection_Level> GetList(int StrategyId)
        {
            if (StrategyId==0)
                return GetAll();

            List<Strategy_Suspection_Level> strategy_Suspection_level = new List<Strategy_Suspection_Level>();
            try
            {
                using (Entities context = new Entities())
                {
                    strategy_Suspection_level = context.Strategy_Suspection_Level
                        .Where(s =>
                            (s.StrategyId == StrategyId)
                        )
                        .Include(s => s.Strategy)
                        .Include(s => s.Suspection_Level)
                        .OrderBy(s => new { s.StrategyId,s.LevelId,s.CriteriaId1, s.CriteriaId2, s.CriteriaId3, s.CriteriaId4, s.CriteriaId5, s.CriteriaId6 })
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Suspection_level.GetList()", err);
            }
            return strategy_Suspection_level;
        }
        //----------------------------------------
        public static List<Strategy_Suspection_Level> GetAll()
        {
            List<Strategy_Suspection_Level> strategy_Suspection_level = new List<Strategy_Suspection_Level>();
            try
            {
                using (Entities context = new Entities())
                {
                    strategy_Suspection_level = context.Strategy_Suspection_Level
                        .Include(s => s.Strategy)
                        .Include(s => s.Suspection_Level)
                        .OrderBy(s => new { s.StrategyId, s.LevelId, s.CriteriaId1, s.CriteriaId2, s.CriteriaId3, s.CriteriaId4, s.CriteriaId5, s.CriteriaId6 })
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Suspection_level.GetAll()", err);
            }
            return strategy_Suspection_level;
        }
       
        //----------------------------------------
        public static bool Save(Strategy_Suspection_Level strategy_Suspection_level)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    if (strategy_Suspection_level.Id == 0)
                    {
                        context.Strategy_Suspection_Level.Add(strategy_Suspection_level);
                    }
                    else
                    {
                        context.Entry(strategy_Suspection_level).State = System.Data.EntityState.Modified;

                    }
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Suspection_level.Save(Id: " + strategy_Suspection_level.Id + ")", err);
            }
            return success;
        }

        //-------------------------------------
        public static bool Delete(int id)
        {
            Strategy_Suspection_Level strategy_Suspection_Level = new Strategy_Suspection_Level() { Id = id };
            return Delete(strategy_Suspection_Level);
        }
        //-------------------------------------
        private static bool Delete(Strategy_Suspection_Level strategy_Suspection_Level)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    context.Entry(strategy_Suspection_Level).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                    success = true;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Suspection_Level.Delete(Id: " + strategy_Suspection_Level.Id + ")", err);
            }
            return success;
        }
        //-------------------------------------










    }
}
