using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class RouteManager
    {
        public void ConvertRouteOptions(List<SupplierRoute> options, out string suppliersOption, out string suppliersOrderOption, out string percentagesOption)
        {
            suppliersOption = null;
            suppliersOrderOption = null;
            percentagesOption = null;
            if(options != null && options.Count > 0)
            {
                var orderedOptions = options.OrderBy(itm => itm.SupplierId).ToList();
                StringBuilder suppliersOptionBuilder = new StringBuilder();
                
                bool isDefaultOrder = true;
                bool isDefaultPercentage = true;
                int index = 0;
                foreach(var option in orderedOptions)
                {
                    if (suppliersOptionBuilder == null)
                        suppliersOptionBuilder = new StringBuilder();
                    else
                        suppliersOptionBuilder.Append(',');
                    suppliersOptionBuilder.Append(option.SupplierId);

                    if (options[index] != option)
                        isDefaultOrder = false;
                    if (option.Percentage.HasValue)
                        isDefaultPercentage = false;
                    index++;
                }
                suppliersOption = suppliersOptionBuilder.ToString();

                if (!isDefaultOrder || !isDefaultPercentage)
                {
                    StringBuilder suppliersOrderOptionBuilder = null;
                    StringBuilder percentagesOptionBuilder = null;
                    foreach(var option in options)
                    {
                        if(!isDefaultOrder)
                        {
                            if (suppliersOrderOptionBuilder == null)
                                suppliersOrderOptionBuilder = new StringBuilder();
                            else
                                suppliersOrderOptionBuilder.Append(',');
                            suppliersOrderOptionBuilder.Append(orderedOptions.IndexOf(option));
                        }
                        if (!isDefaultPercentage)
                        {
                            if (percentagesOptionBuilder == null)
                                percentagesOptionBuilder = new StringBuilder();
                            else
                                percentagesOptionBuilder.Append(',');
                            percentagesOptionBuilder.Append(option.Percentage);
                        }
                    }
                    if (!isDefaultOrder)
                        suppliersOrderOption = suppliersOrderOptionBuilder.ToString();
                    if (!isDefaultPercentage)
                        percentagesOption = percentagesOptionBuilder.ToString();
                }
            }
        }
    }
}
