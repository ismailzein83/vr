using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class CodeController:Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public string GetCodes(int zoneID, DateTime effectiveOn)
        {

            CodeManager manager = new CodeManager();
            List<Code> result = manager.GetCodes(zoneID, effectiveOn);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            long previous = long.MinValue;
            bool insideRange = false;
            foreach (Code code in result)
            {
                long current = 0;

                try
                {
                    current = long.Parse(code.Value);
                    if (current == (previous + 1) && code.EndEffectiveDate == null)
                    {
                        if (!insideRange)
                        {
                            insideRange = true;
                        }
                    }
                    else
                    {
                        if (insideRange)
                        {
                            sb.Append("-");
                            sb.Append(previous);
                            sb.Append(";");
                            sb.AppendFormat("{0}{1}", current, code.EndEffectiveDate.HasValue ? string.Format("*({0:yyyy-MM-dd})", code.EndEffectiveDate) : string.Empty);
                        }
                        else
                        {
                            if (sb.Length > 0) sb.Append(";");
                            sb.AppendFormat("{0}{1}", current, code.EndEffectiveDate.HasValue ? string.Format("*({0:yyyy-MM-dd})", code.EndEffectiveDate) : string.Empty);
                        }
                        insideRange = false;
                    }
                    previous = current;
                }
                catch
                {
                    if (sb.Length > 0) sb.Append(";");
                    sb.AppendFormat("{0}{1}", code.Value, code.EndEffectiveDate.HasValue ? string.Format("*({0:yyyy-MM-dd})", code.EndEffectiveDate) : string.Empty);
                    previous = long.MinValue;
                }
            }
            if (insideRange && previous != long.MinValue)
            {
                sb.Append("-");
                sb.Append(previous);
            }
            return sb.ToString();
        }
    }
}