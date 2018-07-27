using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
	public class CDRCorrelationDefinitionManager
	{
		#region Public Methods
		public CDRCorrelationDefinition GetCDRCorrelationDefinition(Guid cdrCorrelationDefinitionId)
		{
            var cdrCorrelationDefinitions = GetCachedCDRCorrelationDefinitions();
            return cdrCorrelationDefinitions.FindRecord(x => x.VRComponentTypeId == cdrCorrelationDefinitionId);
		}

        public IEnumerable<CDRCorrelationDefinitionInfo> GetCDRCorrelationDefinitionsInfo(CDRCorrelationDefinitionInfoFilter filter)
		{
            Func<CDRCorrelationDefinition, bool> filterExpression = null;
			if (filter != null)
			{
				filterExpression = (item) =>
				{
					return true;
				};
			}

            return GetCachedCDRCorrelationDefinitions().MapRecords(CDRCorrelationDefinitionInfoMapper, filterExpression);
		}

		#endregion

		#region Private Methods

        private Dictionary<Guid, CDRCorrelationDefinition> GetCachedCDRCorrelationDefinitions()
		{
			VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
            return vrComponentTypeManager.GetCachedComponentTypes<CDRCorrelationDefinitionSettings, CDRCorrelationDefinition>();
		}

		#endregion

		#region Mappers

        private CDRCorrelationDefinitionInfo CDRCorrelationDefinitionInfoMapper(CDRCorrelationDefinition cdrCorrelationDefinition)
		{
            return new CDRCorrelationDefinitionInfo
			{
                CDRCorrelationDefinitionId = cdrCorrelationDefinition.VRComponentTypeId,
				Name = cdrCorrelationDefinition.Name,
			};
		}

		#endregion
	}
}
