using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierProfileBE360DegreeNodeType : BE360DegreeNodeTypeExtendedSettings
    {
        static CarrierProfileManager s_carrierProfileManager = new CarrierProfileManager();
        static BE360DegreeManager s_be360DegreeManager = new BE360DegreeManager();
        
        public override List<BE360DegreeNode> GetChildNodes(IBE360DegreeNodeGetChildNodesContext context)
        {
            List<BE360DegreeNode> childNodes = new List<BE360DegreeNode>();
            List<BE360DegreeNodeType<CarrierProfileChildBE360DegreeNodeTypeSettings>> childNodeTypes = s_be360DegreeManager.GetNodeTypesOfT<CarrierProfileChildBE360DegreeNodeTypeSettings>();
            int carrierProfileId = context.Node.EntityId.CastWithValidateStruct<int>("context.Node.EntityId");
            foreach (var childNodeType in childNodeTypes)
            {
                childNodeType.NodeType.ThrowIfNull("childNodeType.NodeType", childNodeType.NodeType.VRComponentTypeId);
                childNodeType.ExtendedSettings.ThrowIfNull("childNodeType.ExtendedSettings", childNodeType.NodeType.VRComponentTypeId);
                var createChildNodeContext = new CarrierProfileChildBE360DegreeNodeTypeCreateNodeContext { CarrierProfileId = carrierProfileId };
                childNodes.Add(childNodeType.ExtendedSettings.CreateNode(createChildNodeContext));
            }
            return childNodes;
        }

        public override BE360DegreeNode RefreshNode(IBE360DegreeNodeRefreshNodeContext context)
        {
            int carrierProfileId = context.Node.EntityId.CastWithValidateStruct<int>("context.Node.EntityId");
            return CreateNode(carrierProfileId);
        }

        public BE360DegreeNode CreateNode(CarrierProfile carrierProfile)
        {
            return new BE360DegreeNode
            {
                EntityId = carrierProfile.CarrierProfileId,
                Title = s_carrierProfileManager.GetCarrierProfileName(carrierProfile)
            };
        }

        public BE360DegreeNode CreateNode(int carrierProfileId)
        {
            var carrierProfile = s_carrierProfileManager.GetCarrierProfile(carrierProfileId);
            carrierProfile.ThrowIfNull("carrierProfile", carrierProfileId);
            return CreateNode(carrierProfile);
        }

        #region Private Classes

        private class CarrierProfileChildBE360DegreeNodeTypeCreateNodeContext : ICarrierProfileChildBE360DegreeNodeTypeCreateNodeContext
        {
            public int CarrierProfileId
            {
                get;
                set;
            }
        }

        #endregion
    }

    public abstract class CarrierProfileChildBE360DegreeNodeTypeSettings : BE360DegreeNodeTypeExtendedSettings
    {
        public abstract BE360DegreeNode CreateNode(ICarrierProfileChildBE360DegreeNodeTypeCreateNodeContext context);
    }

    public interface ICarrierProfileChildBE360DegreeNodeTypeCreateNodeContext
    {
        int CarrierProfileId { get; }
    }
}
