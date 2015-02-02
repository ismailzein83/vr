using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.CommonLibrary;


namespace Vanrise.Fzero.Bypass
{
    public partial class ValueType
    {
        public static List<ValueType> GetValueTypes()
        {
            List<ValueType> ValueTypesList = new List<ValueType>() ;

            try
            {
                using (Entities context = new Entities())
                {
                    ValueTypesList = context.ValueTypes
                        .OrderBy(x => x.Name) .ToList();
                }
            }
            catch (Exception err)
            {
                FileLogger.Write("Error in Vanrise.Fzero.Bypass.ValueType.GetAllValueTypes()", err);
            }


            return ValueTypesList;
        }
    }
}
