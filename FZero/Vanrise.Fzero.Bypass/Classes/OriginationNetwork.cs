using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class OriginationNetwork
    {
        public static List<OriginationNetwork> GetAllOriginationNetworks()
        {
            List<OriginationNetwork> OriginationNetworksList = new List<OriginationNetwork>();

            try
            {
                using (Entities context = new Entities())
                {
                    OriginationNetworksList = context.OriginationNetworks
                                            .OrderByDescending(u => u.Name)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.OriginationNetwork.GetOriginationNetworks()", err);
            }

            return OriginationNetworksList;
        }

        public static bool SaveBulk(string tableName, List<OriginationNetwork> listOriginationNetworks)
        {
            bool success = false;
            try
            {
                Manager.InsertData(listOriginationNetworks.ToList(), tableName, "FMSConnectionString");
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.OriginationNetwork.SaveBulk(" + listOriginationNetworks.Count.ToString() + ")", err);
            }
            return success;
        }
    }
}
