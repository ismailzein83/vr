using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.MainExtensions
{
    public class VRActionAuditLKUPBEDefinitionSettings : LKUPBEDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("442BCFAD-1407-4158-82E7-E1B7A0AB458B"); }
        }
        public VRActionAuditLKUPType Type { get; set; }
        public override Dictionary<string, LKUPBusinessEntityItem> GetAllLKUPBusinessEntityItems(ILKUPBusinessEntityExtendedSettingsContext context)
        {
            VRActionAuditLKUPManager actionAuditManager = new VRActionAuditLKUPManager();
            Dictionary<string, LKUPBusinessEntityItem> LKUPEntityItems = new Dictionary<string, LKUPBusinessEntityItem>();
            var lookUps = actionAuditManager.GetAllLKUPItems();
            if (lookUps != null)
            {
                foreach (var item in lookUps)
                {
                    if (item != null)
                    {
                        LKUPBusinessEntityItem lkUpBEItem = new LKUPBusinessEntityItem
                        {
                            LKUPBusinessEntityItemId = item.VRActionAuditLKUPId.ToString(),
                            Name = item.Name
                        };
                        switch (this.Type)
                        {
                            case VRActionAuditLKUPType.URL:
                                lkUpBEItem.BusinessEntityDefinitionId = Guid.Parse("1D4C92FB-AA45-4015-8C6D-A8951B0E5575");
                                break;
                            case VRActionAuditLKUPType.Module:
                                lkUpBEItem.BusinessEntityDefinitionId = Guid.Parse("D85CC4AA-36A5-413F-A19A-0BE0DA9E7FDA");
                                break;
                            case VRActionAuditLKUPType.Entity:
                                lkUpBEItem.BusinessEntityDefinitionId = Guid.Parse("310C3F65-3F22-47D7-8D99-763ECCF98B81");
                                break;
                            case VRActionAuditLKUPType.Action:
                                lkUpBEItem.BusinessEntityDefinitionId = Guid.Parse("56693F28-C36F-48DA-82BD-B4B9DF018858");
                                break;
                        }
                        LKUPEntityItems.Add(item.VRActionAuditLKUPId.ToString(), lkUpBEItem);
                    }
                }
            }
            return LKUPEntityItems;
        }

        public override bool IsCacheExpired(object parameter, ref DateTime? lastCheckTime)
        {
             throw new NotImplementedException();
        }
    }
}
