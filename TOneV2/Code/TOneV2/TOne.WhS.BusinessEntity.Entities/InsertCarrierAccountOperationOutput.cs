using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
	public class InsertCarrierAccountOperationOutput<T> : Vanrise.Entities.InsertOperationOutput<T>
	{
		public List<string> ValidationMessages { get; set; }
	}
}
