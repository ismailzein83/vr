﻿using System;
using System.Activities;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
	public class SetReceivedPricelistAsCompleted : CodeActivity
	{
		[RequiredArgument]
		public InArgument<int> ReceivedPricelistRecordId { get; set; }

		[RequiredArgument]
		public InArgument<int> PricelistId { get; set; }

		protected override void Execute(CodeActivityContext context)
		{
			int receivedPricelistRecordId = this.ReceivedPricelistRecordId.Get(context);
			int pricelistId = this.PricelistId.Get(context);
			IReceivedPricelistManagerTemp manager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManagerTemp>();

			manager.SetReceivedPricelistAsCompleted(receivedPricelistRecordId, ReceivedPricelistStatus.Succeeded, pricelistId);
		}
	}
}
