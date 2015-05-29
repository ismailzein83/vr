using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using Vanrise.CommonLibrary;
using Vanrise.Fzero.MobileCDRAnalysis.Providers;


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

                    strategy = context.Strategies.Include(x => x.Strategy_Min_Values).Include(x => x.Strategy_Suspicion_Level).Include(x => x.StrategyPeriods).Include(x => x.StrategyThresholds)
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



       



        public static List<Strategy> GetList(string name, string Description, int UserID, bool IsSuperUser)
        {

            List<Strategy> strategies = new List<Strategy>();
            try
            {
                using (Entities context = new Entities())
                {
                    strategies = context.Strategies
                        //.Include(s => s.Switch_DatabaseConnection)
                        .Where(s =>
                            ((s.Name.Contains(name)) && s.Description.Contains(Description) && (s.UserId == UserID || IsSuperUser))
                           
                            
                        )
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy.GetList()", err);
            }
            return strategies;
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


 public static bool Delete(int id)
        {
            Strategy strategy = new Strategy() { Id = id};
            return Delete(strategy);
        }

        private static bool Delete(Strategy strategy)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {

                    Strategy st = Strategy.Load(strategy.Id);

                    foreach (var i in st.StrategyPeriods.ToList())
                        context.Entry(i).State = System.Data.EntityState.Deleted;

                    foreach (var i in st.Strategy_Min_Values.ToList())
                        context.Entry(i).State = System.Data.EntityState.Deleted;

                    foreach (var i in st.Strategy_Suspicion_Level.ToList())
                        context.Entry(i).State = System.Data.EntityState.Deleted;

                    foreach (var i in st.StrategyThresholds.ToList())
                        context.Entry(i).State = System.Data.EntityState.Deleted;


                    context.Entry(st).State = System.Data.EntityState.Deleted;


                 
                    context.SaveChanges();
                    success = true;
                }
            }
            catch(Exception err)
            {
                FileLogger.Write("DataLayer.Strategy.Delete(Id: " + strategy.Id + ")", err);
            }
            return success;
        }


        public static bool IsFullNameUsed(Strategy strategy)
        {
            bool isUsed = false;
            try
            {
                using (Entities context = new Entities())
                {
                    isUsed = context.Strategies
                        .Where(p => p.Name == strategy.Name
                            && p.Id != strategy.Id)
                        .Count() > 0;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy.IsFullNameUnique(Id: " + strategy.Id + ", Name: " + strategy.Name + ")", err);
            }
            return isUsed;
        }


        public static bool Save(Strategy strategy)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    if (strategy.Id == 0)
                    {
                        List<Criteria_Profile> criteria = new List<Criteria_Profile>();
                        criteria = Criteria_Profile.GetAll();

                        List<StrategyThreshold> ListStrategyThreshold = new List<StrategyThreshold>();

                        foreach (var i in criteria)
                        {
                             StrategyThreshold temp =new StrategyThreshold();
                             temp.CriteriaID = i.Id;
                             temp.MaxValue = 0;
                             ListStrategyThreshold.Add(temp);
                        }


                        strategy.StrategyThresholds = ListStrategyThreshold;










                        List<StrategyPeriod> ListStrategyPeriod = new List<StrategyPeriod>();

                        foreach (var i in criteria)
                        {
                            StrategyPeriod temp = new StrategyPeriod();
                            temp.CriteriaID = i.Id;
                            temp.Value = 0;
                            ListStrategyPeriod.Add(temp);
                        }


                        strategy.StrategyPeriods = ListStrategyPeriod;







                        context.Strategies.Add(strategy);
                        context.SaveChanges();
                        
                    }
                    else
                    {
                        context.Entry(strategy).State = System.Data.EntityState.Modified;
                        context.SaveChanges();
                    }
                    

                   

                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.Strategy.Save(Id: " + strategy.Id + ")", err);
            }
            return success;
        }






    }
}
