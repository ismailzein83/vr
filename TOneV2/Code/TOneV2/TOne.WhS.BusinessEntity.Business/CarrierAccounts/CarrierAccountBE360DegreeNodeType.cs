using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierAccountBE360DegreeNodeType : BE360DegreeNodeTypeExtendedSettings
    {
        static CarrierAccountManager s_carrierAccountManager = new CarrierAccountManager();
        static BE360DegreeManager s_be360DegreeManager = new BE360DegreeManager();

        public override List<BE360DegreeNode> GetChildNodes(IBE360DegreeNodeGetChildNodesContext context)
        {
            List<BE360DegreeNode> childNodes = new List<BE360DegreeNode>();
            List<BE360DegreeNodeType<CarrierAccountChildBE360DegreeNodeTypeSettings>> childNodeTypes = s_be360DegreeManager.GetNodeTypesOfT<CarrierAccountChildBE360DegreeNodeTypeSettings>();
            int carrierAccountId = context.Node.EntityId.CastWithValidateStruct<int>("context.Node.EntityId");
            foreach (var childNodeType in childNodeTypes)
            {
                childNodeType.NodeType.ThrowIfNull("childNodeType.NodeType", childNodeType.NodeType.VRComponentTypeId);
                childNodeType.ExtendedSettings.ThrowIfNull("childNodeType.ExtendedSettings", childNodeType.NodeType.VRComponentTypeId);
                var createChildNodeContext = new CarrierAccountChildBE360DegreeNodeTypeCreateNodeContext { CarrierAccountId = carrierAccountId };
                childNodes.Add(childNodeType.ExtendedSettings.CreateNode(createChildNodeContext));
            }
            return childNodes;
        }

        public override BE360DegreeNode RefreshNode(IBE360DegreeNodeRefreshNodeContext context)
        {
            int carrierAccountId = context.Node.EntityId.CastWithValidateStruct<int>("context.Node.EntityId");
            return CreateNode(carrierAccountId);
        }

        public BE360DegreeNode CreateNode(CarrierAccount carrierAccount)
        {
            return new BE360DegreeNode
            {
                EntityId = carrierAccount.CarrierAccountId,
                Title = String.Format("{0} ({1})", carrierAccount.NameSuffix, carrierAccount.AccountType)
            };
        }

        public BE360DegreeNode CreateNode(int carrierAccountId)
        {
            var carrierAccount = s_carrierAccountManager.GetCarrierAccount(carrierAccountId);
            carrierAccount.ThrowIfNull("carrierAccount", carrierAccountId);
            return CreateNode(carrierAccount);
        }

        #region Private Classes

        private class CarrierAccountChildBE360DegreeNodeTypeCreateNodeContext : ICarrierAccountChildBE360DegreeNodeTypeCreateNodeContext
        {
            public int CarrierAccountId
            {
                get;
                set;
            }
        }

        #endregion
    }

    public abstract class CarrierAccountChildBE360DegreeNodeTypeSettings : BE360DegreeNodeTypeExtendedSettings
    {
        public abstract BE360DegreeNode CreateNode(ICarrierAccountChildBE360DegreeNodeTypeCreateNodeContext context);
    }

    public interface ICarrierAccountChildBE360DegreeNodeTypeCreateNodeContext
    {
        int CarrierAccountId { get; }
    }
}
