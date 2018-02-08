using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierFaultTicketsSettings : GenericBEExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("FCFA0193-9463-4435-9D96-54FD2B6D050B"); }
        }
        public override object GetInfoByType(IGenericBEExtendedSettingsContext context)
        {
            if (context.InfoType == null)
                return null;

            switch (context.InfoType)
            {
                case "SerialNumberPattern": return new ConfigManager().GetFaultTicketsSupplierSerialNumberPattern();
                case "SerialNumberInitialSequence": return new ConfigManager().GetFaultTicketsSupplierSerialNumberInitialSequence();
                default: return null;
            }
        }
    }
}
