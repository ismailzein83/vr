using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierProfileAccountsBE360DegreeNodeType : CarrierProfileChildBE360DegreeNodeTypeSettings
    {
        static CarrierAccountManager s_carrierAccountManager = new CarrierAccountManager();
        static BE360DegreeManager s_be360DegreeManager = new BE360DegreeManager();


        public override List<BE360DegreeNode> GetChildNodes(IBE360DegreeNodeGetChildNodesContext context)
        {
            int carrierProfileId = context.Node.EntityId.CastWithValidateStruct<int>("context.Node.EntityId");
            BE360DegreeNodeType<CarrierAccountBE360DegreeNodeType> carrierAccountNodeType = s_be360DegreeManager.GetFirstNodeTypeOfT<CarrierAccountBE360DegreeNodeType>();
            carrierAccountNodeType.ThrowIfNull("carrierAccountNodeType");
            carrierAccountNodeType.ExtendedSettings.ThrowIfNull("carrierAccountNodeType.ExtendedSettings");
            List<BE360DegreeNode> childNodes = new List<BE360DegreeNode>();
            var carrierAccounts = s_carrierAccountManager.GetCarriersByProfileId(carrierProfileId, true, true);
            if (carrierAccounts != null)
                childNodes.AddRange(carrierAccounts.Select(ca => carrierAccountNodeType.ExtendedSettings.CreateNode(ca)));
            return childNodes;
        }

        public override BE360DegreeNode RefreshNode(IBE360DegreeNodeRefreshNodeContext context)
        {
            int carrierProfileId = context.Node.EntityId.CastWithValidateStruct<int>("context.Node.EntityId");
            return CreateNode(carrierProfileId);
        }

        public override BE360DegreeNode CreateNode(ICarrierProfileChildBE360DegreeNodeTypeCreateNodeContext context)
        {
            return CreateNode(context.CarrierProfileId);
        }

        private BE360DegreeNode CreateNode(int carrierProfileId)
        {
            var carrierAccounts = s_carrierAccountManager.GetCarriersByProfileId(carrierProfileId, true, true);
            int carrierAccountsCount = carrierAccounts != null ? carrierAccounts.Count() : 0;
            return new BE360DegreeNode
            {
                EntityId = carrierProfileId,
                Title = String.Format("Accounts ({0})", carrierAccountsCount)
            };
        }
    }
}
