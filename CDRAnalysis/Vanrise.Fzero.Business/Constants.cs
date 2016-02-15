using System.Configuration;
using Vanrise.Fzero.Entities;

namespace Vanrise.Fzero.Business
{
    public static class GlobalConstants
    {
        public static OperatorType _DefaultOperatorType = (ConfigurationManager.AppSettings["OperatorType"] == "1" ? OperatorType.Mobile : OperatorType.PSTN);
    }
}
