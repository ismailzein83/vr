using System;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
	public class VRCorrelationDefinitionSettings : VRComponentTypeSettings
	{
		public override Guid VRComponentTypeConfigId
		{
			get { return new Guid("919798ED-8B0D-40CD-A011-D90C3B691C88"); }
		}
		public Guid MergeDataTransformationDefinitionId { get; set; }

		public Guid InputDataRecordTypeId { get; set; }

		public Guid OutputDataRecordTypeId { get; set; }

		public Guid InputDataRecordStorageId { get; set; }

		public Guid OutputDataRecordStorageId { get; set; }

		public string CallingNumberFieldName { get; set; }

		public string CalledNumberFieldName { get; set; }

		public string DurationFieldName { get; set; }

		public string DatetimeFieldName { get; set; }
	}
}