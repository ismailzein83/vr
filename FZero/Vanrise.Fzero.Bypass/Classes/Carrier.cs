using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class Carrier
    {
        public static List<Carrier> GetAllCarriers()
        {
            List<Carrier> CarriersList = new List<Carrier>();

            try
            {
                using (Entities context = new Entities())
                {
                    CarriersList = context.Carriers
                                            .OrderByDescending(u => u.Name)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Carrier.GetCarriers()", err);
            }

            return CarriersList;
        }

        public static bool SaveBulk(string tableName, List<Carrier> listCarriers)
        {
            bool success = false;
            try
            {
                Manager.InsertData(listCarriers.ToList(), tableName, "FMSConnectionString");
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.Carrier.SaveBulk(" + listCarriers.Count.ToString() + ")", err);
            }
            return success;
        }
    }
}
