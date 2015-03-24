using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class SysParameter
    {
        public static SysParameter Load(int id)
        {
            SysParameter sysParameter = null;
            try
            {
                using (Entities context = new Entities())
                {
                    sysParameter = context.SysParameters
                        .Find(id);
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.SysParameter.Load(" + id.ToString() + ")", err);
            }
            return sysParameter == null ? new SysParameter() : sysParameter;
        }

        public static List<SysParameter> GetSysParameters(string text)
        {
            List<SysParameter> SysParametersList= new List<SysParameter>();

            try
            {
              using (Entities context = new Entities())
                {
                    SysParametersList = context.SysParameters
                        .Include(x=>x.ValueType)
                        .Where(x => x.Name.Contains(text) || x.Description.Contains(text))
                        .OrderBy(x => x.Name)
                        .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.SysParameter.GetSysParameters(" + text + ")", err);
            }


            return SysParametersList;
        }

        public static bool Save(SysParameter SysParameter)
        {
            bool success = false;
            try
            {
                using (Entities context = new Entities())
                {
                    SysParameter.LastUpdateDate = DateTime.Now;
                    context.Entry(SysParameter).State = System.Data.EntityState.Modified;
                    context.SaveChanges();
                    success = true;
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.SysParameter.Save(" + SysParameter.ID.ToString() + ")", err);
            }
            return success;
        }

        #region Parameters Values
        
        
        public static int Global_GridPageSize { get { return Load(1).Value.ToInt(); } }
        public static int Global_GMT { get { return Load(7).Value.ToInt(); } }
        public static string Global_DefaultMobileOperator { get { return Load(9).Value; } }
        public static string Global_SenderEmail { get { return Load(10).Value; } }
        public static int Suspect_Low { get { return Load(13).Value.ToInt(); } }
        public static int Suspect_Middle_From { get { return Load(14).Value.ToInt(); } }
        public static int Suspect_Middle_To { get { return Load(15).Value.ToInt(); } }
        public static int Suspect_High { get { return Load(16).Value.ToInt(); } }

        #endregion
    }
}
