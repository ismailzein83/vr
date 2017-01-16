using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class BEParentChildRelationDefinitionManager
    {
        #region Public Methods

        public IEnumerable<BEParentChildRelationDefinition> GetBEParentChildRelationDefinitions()
        {
            VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetComponentTypes<BEParentChildRelationDefinitionSettings, BEParentChildRelationDefinition>();
        }

        public BEParentChildRelationDefinition GetBEParentChildRelationDefinition(Guid beParentChildRelationDefinitionId)
        {
            var packageDefinitions = GetBEParentChildRelationDefinitions();
            return packageDefinitions.FindRecord(x => x.VRComponentTypeId == beParentChildRelationDefinitionId);
        }

        public IEnumerable<BEParentChildRelationDefinitionInfo> GetBEParentChildRelationDefinitionsInfo(BEParentChildRelationDefinitionFilter filter)
        {
            Func<BEParentChildRelationDefinition, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (beParentChildRelationDefinition) =>
                    {
                        if (filter.ParentBEDefinitionId != Guid.Empty && filter.ParentBEDefinitionId != beParentChildRelationDefinition.Settings.ParentBEDefinitionId)
                            return false;

                        if (filter.ChildBEDefinitionId != Guid.Empty && filter.ChildBEDefinitionId != beParentChildRelationDefinition.Settings.ChildBEDefinitionId)
                            return false;

                        return true;
                    };
            }

            var beParentChildRelationDefinitions = GetBEParentChildRelationDefinitions();
            return beParentChildRelationDefinitions.MapRecords(BEParentChildRelationDefinitionInfoMapper, filterExpression);
        }

        public IEnumerable<string> GetBEParentChildRelationGridColumnNames(Guid beParentChildRelationDefinitionId)
        {
            List<string> beParentChildRelationGridColumnNames = new List<string>();
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new BusinessEntityDefinitionManager();

            BEParentChildRelationDefinition beParentChildRelationDefinition = this.GetBEParentChildRelationDefinition(beParentChildRelationDefinitionId);
            if (beParentChildRelationDefinition == null)
                throw new NullReferenceException(string.Format("beParentChildRelationDefinition {0}", beParentChildRelationDefinitionId));

            if (beParentChildRelationDefinition.Settings == null)
                throw new NullReferenceException(string.Format("beParentChildRelationDefinition.Settings {0}", beParentChildRelationDefinitionId));

            beParentChildRelationGridColumnNames.Add(businessEntityDefinitionManager.GetBusinessEntityDefinitionName(beParentChildRelationDefinition.Settings.ParentBEDefinitionId));
            beParentChildRelationGridColumnNames.Add(businessEntityDefinitionManager.GetBusinessEntityDefinitionName(beParentChildRelationDefinition.Settings.ChildBEDefinitionId));

            return beParentChildRelationGridColumnNames;
        }

        #endregion

        #region Mapper

        public BEParentChildRelationDefinitionInfo BEParentChildRelationDefinitionInfoMapper(BEParentChildRelationDefinition beParentChildRelationDefinition)
        {
            return new BEParentChildRelationDefinitionInfo
            {
                Name = beParentChildRelationDefinition.Name,
                BEParentChildRelationDefinitionId = beParentChildRelationDefinition.VRComponentTypeId,
                ParentBEDefinitionId = beParentChildRelationDefinition.Settings.ParentBEDefinitionId,
                ChildBEDefinitionId = beParentChildRelationDefinition.Settings.ChildBEDefinitionId
            };
        }

        #endregion
    }
}
