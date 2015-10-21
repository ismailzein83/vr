using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.Entities;

namespace Vanrise.Fzero.Business
{
    public static class GlobalConstants
    {
        public static OperatorType _DefaultOperatorType = (ConfigurationManager.AppSettings["OperatorType"] == "1" ? OperatorType.Mobile : OperatorType.PSTN);
    }
}
