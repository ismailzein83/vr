using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;


namespace Vanrise.Fzero.Bypass
{
    public partial class View_Origination
    {
        public static List<View_Origination> GetView_Origination(int ClientID, int MobileOperatorID, DateTime? FromAttemptDateTime, DateTime? ToAttemptDateTime)
        {
            List<View_Origination> listView_Origination = new List<View_Origination>();
            try
            {
                using (Entities context = new Entities())
                {
                    var _ClientID = new SqlParameter("@ClientID", ClientID);
                    var _MobileOperatorID = new SqlParameter("@MobileOperatorID", MobileOperatorID);
                    var _FromAttemptDateTime = new SqlParameter("@StartDate", FromAttemptDateTime);
                    var _ToAttemptDateTime = new SqlParameter("@EndDate", ToAttemptDateTime);

                    listView_Origination = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<View_Origination>("prViewOrigination @MobileOperatorID, @StartDate, @EndDate, @ClientID", _MobileOperatorID, _FromAttemptDateTime, _ToAttemptDateTime, _ClientID).ToList();



                    listView_Origination = (from item in listView_Origination
                     where item.AttemptDateTime >= FromAttemptDateTime && item.AttemptDateTime <= ToAttemptDateTime
                     group item by item.OriginationNetwork into grp
                     select new View_Origination
                     {
                         OriginationNetwork = grp.Key,
                         count = grp.Sum(x => x.count)
                     }).ToList();

                }
            }
            catch (Exception err)
            {
                throw err;
            }
            return listView_Origination;
        }
    }
}
