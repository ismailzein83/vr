using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
	public class UpdateCarrierAccountOperationOutput<T> : Vanrise.Entities.UpdateOperationOutput<T>
	{
		public List<string> ValidationMessages { get; set; }
	}
}
