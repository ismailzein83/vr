using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Vanrise.CommonLibrary;


namespace Vanrise.Fzero.Bypass
{
    public partial class LoggedAction
    {
        public static bool AddLoggedAction(int actionTypeID, int userId)
        {
            LoggedAction action = new LoggedAction() { ActionTypeID = actionTypeID, ActionBy = userId, LogDate= DateTime.Now };
            return Save(action);
        }
        public static bool Save(LoggedAction loggedAction)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    context.LoggedActions.Add(loggedAction);
                    context.SaveChanges();
                    success = true;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.LoggedAction.Save(" + loggedAction.ID.ToString() + ")", err);
            }
            return success;
        }

        public static List<LoggedAction> GetLoggedActions(int? actionTypeID,  int? actionBy, DateTime? fromDate, DateTime? toDate)
        {
            List<LoggedAction> LoggedActionsList = new List<LoggedAction>();

            try
            {
                using (Entities context = new Entities())
                {

                    LoggedActionsList = context.LoggedActions
                                                .Include(u => u.User)
                                                .Include(u => u.ActionType)
                                                .Where(u => u.ID>0
                                                    && (actionBy <= 0 || u.ActionBy == actionBy)
                                                    && (!toDate.HasValue || u.LogDate <= toDate.Value)
                                                    && (!fromDate.HasValue || u.LogDate >= fromDate.Value)
                                                    && (actionTypeID <= 0 || u.ActionTypeID == actionTypeID))
                                                .OrderByDescending(x => x.LogDate)
                                                .ToList();
                    
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.LoggedAction.GetLoggedActions( " + actionTypeID.ToString() + ", " + actionBy.ToString() + ",  " + fromDate.ToString() + ", " + toDate.ToString() + "  )", err);
            }


            return LoggedActionsList;
        }

        public static bool Delete(List<int> ListIds)
        {

            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {

                    foreach (int id in ListIds)
                    {
                        foreach (LoggedAction loggedAction in context.LoggedActions.Where(x => x.ID == id).ToList())
                        {
                            context.Entry(loggedAction).State = System.Data.EntityState.Deleted;
                        }
                    }

                    context.SaveChanges();
                }
                success = true;
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.UserPermission.Delete(" + ListIds.Count.ToString() + ")", err);
            }
            return success;




        }


    }
}
