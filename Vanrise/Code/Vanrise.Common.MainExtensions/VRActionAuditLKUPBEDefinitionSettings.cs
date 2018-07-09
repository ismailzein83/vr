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
                                lkUpBEItem.BusinessEntityDefinitionId = new Guid("527083e4-f795-46ae-a17e-cd2cff13e7a6");
                                break;
                            case VRActionAuditLKUPType.Module:
                                lkUpBEItem.BusinessEntityDefinitionId = new Guid("a1eb7032-2d31-4f4e-8cec-5dddd14f4e17");
                                break;
                            case VRActionAuditLKUPType.Entity:
                                lkUpBEItem.BusinessEntityDefinitionId = new Guid("2da03147-fe67-4cda-979e-c4abadc35079");
                                break;
                            case VRActionAuditLKUPType.Action:
                                lkUpBEItem.BusinessEntityDefinitionId = new Guid("9a09b2b1-dd18-49c9-9913-3ba97916a1cb");
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
