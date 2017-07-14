using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;

namespace Vanrise.Common.Business
{
    public class BE360DegreeManager
    {
        static VRComponentTypeManager s_componentTypeManager = new VRComponentTypeManager();
        public Dictionary<Guid, BE360DegreeNodeType> GetAllNodeTypes()
        {
            return s_componentTypeManager.GetCachedComponentTypes<BE360DegreeNodeTypeSettings, BE360DegreeNodeType>();
        }

        public BE360DegreeNodeType GetNodeType(Guid nodeConfigId)
        {
            return GetAllNodeTypes().GetRecord(nodeConfigId);
        }

        public List<BE360DegreeNodeType<T>> GetNodeTypesOfT<T>() where T : BE360DegreeNodeTypeExtendedSettings
        {
            var allNodeTypes = GetAllNodeTypes();
            if (allNodeTypes == null)
                return null;
            return allNodeTypes.Values.Where(nodeType => nodeType.Settings != null &&nodeType.Settings.ExtendedSettings != null && nodeType.Settings.ExtendedSettings is T).Select(nodeType => new BE360DegreeNodeType<T>
            {
                NodeType = nodeType,
                ExtendedSettings = (T)nodeType.Settings.ExtendedSettings
            }).ToList();
        }

        public BE360DegreeNodeType<T> GetFirstNodeTypeOfT<T>() where T : BE360DegreeNodeTypeExtendedSettings
        {
            List<BE360DegreeNodeType<T>> nodeTypes = GetNodeTypesOfT<T>();
            if (nodeTypes != null && nodeTypes.Count > 0)
                return nodeTypes.First();
            else
                return null;
        }
    }


}
