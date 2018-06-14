using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.MainExtensions
{
	public class Pop3SupplierPricelistMessageFilter : VRPop3MessageFilter
	{
		public override Guid ConfigId
		{
			get { return new Guid("05382832-5CBB-46C0-8214-B3B81769FB80"); }
		}

		public override bool IsApplicableFunction(VRPop3MailMessageHeader receivedMailMessageHeader)
		{
			var carrierAccountManager = new CarrierAccountManager();
			Dictionary<string, CarrierAccount> supplierAccounts = carrierAccountManager.GetCachedSupplierAccountsByAutoImportEmail();
			var supplierAccount = supplierAccounts.GetRecord(receivedMailMessageHeader.From.ToLower());

			if (supplierAccount == null || supplierAccount.CarrierAccountSettings == null || supplierAccount.CarrierAccountSettings.ActivationStatus == ActivationStatus.Inactive)
				return false;

			if (supplierAccount.SupplierSettings == null || supplierAccount.SupplierSettings.AutoImportSettings == null || supplierAccount.SupplierSettings.AutoImportSettings.IsAutoImportActive == false)
				return false;

			if (supplierAccount.SupplierSettings.AutoImportSettings.SubjectCode == null)
				return false;//Check if we need to throw ex

			if (receivedMailMessageHeader.Subject.ToLower().Contains(supplierAccount.SupplierSettings.AutoImportSettings.SubjectCode.ToLower()))
				return true;

			return false;
		}
	}
}
