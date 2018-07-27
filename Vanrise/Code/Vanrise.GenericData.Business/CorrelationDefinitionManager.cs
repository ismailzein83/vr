using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
	public class CorrelationDefinitionManager
	{
		#region Public Methods
		public VRCorrelationDefinition GetVRCorrelationDefinition(Guid vrCorrelationDefinitionId)
		{
			var vrCorrelationDefinitions = GetCachedVRCorrelationDefinitions();
			return vrCorrelationDefinitions.FindRecord(x => x.VRComponentTypeId == vrCorrelationDefinitionId);
		}

		public IEnumerable<VRCorrelationDefinitionInfo> GetVRCorrelationDefinitionsInfo(VRCorrelationDefinitionInfoFilter filter)
		{
			Func<VRCorrelationDefinition, bool> filterExpression = null;
			if (filter != null)
			{
				filterExpression = (item) =>
				{
					return true;
				};
			}

			return GetCachedVRCorrelationDefinitions().MapRecords(VRCorrelationDefinitionInfoMapper, filterExpression);
		}

		#endregion

		#region Private Methods

		private Dictionary<Guid, VRCorrelationDefinition> GetCachedVRCorrelationDefinitions()
		{
			VRComponentTypeManager vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
			return vrComponentTypeManager.GetCachedComponentTypes<VRCorrelationDefinitionSettings, VRCorrelationDefinition>();
		}

		#endregion

		#region Mappers

		private VRCorrelationDefinitionInfo VRCorrelationDefinitionInfoMapper(VRCorrelationDefinition vrCorrelationDefinition)
		{
			return new VRCorrelationDefinitionInfo
			{
				VRCorrelationDefinitionId = vrCorrelationDefinition.VRComponentTypeId,
				Name = vrCorrelationDefinition.Name,
			};
		}

		#endregion
	}
}
