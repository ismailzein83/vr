using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;


namespace Vanrise.Fzero.Bypass
{
    public partial class ActionType
    {
        public static List<ActionType> GetAllActionTypes()
        {
            List<ActionType> ActionTypesList = new List<ActionType>();

            try
            {
                using (Entities context = new Entities())
                {
                    ActionTypesList = context.ActionTypes
                        .OrderBy(x => x.Name) .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Action.GetAllActionTypes()", err);
            }


            return ActionTypesList;
        }
    }
}
