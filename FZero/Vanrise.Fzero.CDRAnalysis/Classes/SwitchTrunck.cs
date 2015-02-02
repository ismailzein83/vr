using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Vanrise.CommonLibrary;






namespace Vanrise.Fzero.CDRAnalysis
{
    public partial class SwitchTrunck
    {
        public static List<SwitchTrunck> GetAll()
        {
            List<SwitchTrunck> truncks = new List<SwitchTrunck>();
            try
            {
                using (Entities context = new Entities())
                {
                    truncks = context.SwitchTruncks
                        .Include(t => t.SwitchProfile)
                        .Include(t => t.Direction)
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.SwitchTrunck.GetAll()", err);
            }
            return truncks;
        }
       
        public static List<SwitchTrunck> GetList(string trunckName, string switchName)
        {
            if (string.IsNullOrWhiteSpace(trunckName) && string.IsNullOrWhiteSpace(switchName))
                return GetAll();

            List<SwitchTrunck> truncks = new List<SwitchTrunck>();
            try
            {
                using (Entities context = new Entities())
                {
                    truncks = context.SwitchTruncks
                        .Include(t => t.SwitchProfile)
                        .Include(t => t.Direction)
                        .Where(t =>
                            (t.Name.Contains(trunckName))
                            &&
                            (t.SwitchProfile.Name.Contains(switchName))
                        )
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.SwitchTrunck.GetList()", err);
            }
            return truncks;
        }

        public static List<SwitchTrunck> GetList(int switchId)
        {
            List<SwitchTrunck> truncks = new List<SwitchTrunck>();
            try
            {
                using (Entities context = new Entities())
                {
                    truncks = context.SwitchTruncks
                        .Where(t => t.SwitchId == switchId)
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.SwitchTrunck.GetList(switchId: " + switchId + ")", err);
            }
            return truncks;
        }

        public static bool IsSwitchTrunckUsed(SwitchTrunck switchTrunck)
        {
            bool isUsed =false;
            try
            {
                using (Entities context = new Entities())
                {
                    if (switchTrunck.Name != "Unknown")
                        isUsed = context.SwitchTruncks
                         .Where(st => st.Id != switchTrunck.Id
                             && st.Name == switchTrunck.Name
                             && st.SwitchId == switchTrunck.SwitchId)
                         .Count() > 0;

                
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("DataLayer.SwitchTrunck.IsSwitchTrunckUsed()", err);
            }
            return isUsed;
        }
    }
}
