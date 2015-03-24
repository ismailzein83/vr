using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;




namespace Vanrise.Fzero.MobileCDRAnalysis
{
    public partial class Strategy_Suspicion_Level
    {


       // public int SuspectionWeight { get; set; }



        public static Strategy_Suspicion_Level Load(int id)
        {
            Strategy_Suspicion_Level Strategy_Suspicion_Level = new Strategy_Suspicion_Level();
            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    Strategy_Suspicion_Level = context.Strategy_Suspicion_Level
                        .Include(s => s.Strategy)
                        .Where(s => s.Id == id)
                        .OrderBy(s => new { s.StrategyId, s.LevelId, s.CriteriaId1, s.CriteriaId2, s.CriteriaId3, s.CriteriaId4, s.CriteriaId5, s.CriteriaId6 })
                        .FirstOrDefault();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Suspicion_Level.Load(" + id + ")", err);
            }
            return Strategy_Suspicion_Level;
        }
        //----------------------------------------
        public static List<Strategy_Suspicion_Level> GetList(int StrategyId)
        {
            if (StrategyId==0)
                return GetAll();

            List<Strategy_Suspicion_Level> Strategy_Suspicion_Level = new List<Strategy_Suspicion_Level>();
            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    Strategy_Suspicion_Level = context.Strategy_Suspicion_Level
                        .Where(s =>
                            (s.StrategyId == StrategyId)
                        )
                        .Include(s => s.Strategy)
                        .Include(s => s.Suspicion_Level)
                        .OrderBy(s => new { s.StrategyId,s.LevelId,s.CriteriaId1, s.CriteriaId2, s.CriteriaId3, s.CriteriaId4, s.CriteriaId5, s.CriteriaId6 })
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Suspicion_Level.GetList()", err);
            }
            return Strategy_Suspicion_Level;
        }
        //----------------------------------------
        public static List<Strategy_Suspicion_Level> GetAll()
        {
            List<Strategy_Suspicion_Level> Strategy_Suspicion_Level = new List<Strategy_Suspicion_Level>();
            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    Strategy_Suspicion_Level = context.Strategy_Suspicion_Level
                        .Include(s => s.Strategy)
                        .Include(s => s.Suspicion_Level)
                        .OrderBy(s => new { s.StrategyId, s.LevelId, s.CriteriaId1, s.CriteriaId2, s.CriteriaId3, s.CriteriaId4, s.CriteriaId5, s.CriteriaId6 })
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Suspicion_Level.GetAll()", err);
            }
            return Strategy_Suspicion_Level;
        }
       
        //----------------------------------------
        public static bool Save(Strategy_Suspicion_Level Strategy_Suspicion_Level)
        {
            bool success = false;
            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    if (Strategy_Suspicion_Level.Id == 0)
                    {
                        context.Strategy_Suspicion_Level.Add(Strategy_Suspicion_Level);
                    }
                    else
                    {
                        context.Entry(Strategy_Suspicion_Level).State = System.Data.EntityState.Modified;

                    }
                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Suspicion_Level.Save(Id: " + Strategy_Suspicion_Level.Id + ")", err);
            }
            return success;
        }

        //-------------------------------------
        public static bool Delete(int id)
        {
            Strategy_Suspicion_Level Strategy_Suspicion_Level = new Strategy_Suspicion_Level() { Id = id };
            return Delete(Strategy_Suspicion_Level);
        }
        //-------------------------------------
        private static bool Delete(Strategy_Suspicion_Level Strategy_Suspicion_Level)
        {
            bool success = false;
            try
            {
                using (MobileEntities context = new MobileEntities())
                {
                    context.Entry(Strategy_Suspicion_Level).State = System.Data.EntityState.Deleted;
                    context.SaveChanges();
                    success = true;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy_Suspicion_Level.Delete(Id: " + Strategy_Suspicion_Level.Id + ")", err);
            }
            return success;
        }
        //-------------------------------------










    }
}
