using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;

namespace Vanrise.Fzero.Bypass
{
    public partial class AppType
    {
        public static List<AppType> GetAppTypes()
           
        {
            List<AppType> AppTypesList = new List<AppType>();

            try
            {
                using (Entities context = new Entities())
                {
                    AppTypesList = context.AppTypes
                                            .OrderByDescending(u => u.Name)
                                            .ToList();

                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.AppType.GetAppTypes()", err);
            }

            return AppTypesList;
        }
    }
}
