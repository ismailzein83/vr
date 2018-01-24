using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerFaultTicketsSettings : GenericBEExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("3705144E-4BC8-45D8-94D9-96E9AF95353B"); }
        }
        public override object GetInfoByType(IGenericBEExtendedSettingsContext context)
        {
            if (context.InfoType == null)
                return null;

            switch (context.InfoType)
            {
                case "SerialNumberPattern": return "test";
                default: return null;
            }
        }
    }
}
