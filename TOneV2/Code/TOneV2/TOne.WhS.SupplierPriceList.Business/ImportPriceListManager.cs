using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
   public class ImportPriceListManager
    {
       public void InsertPriceListObject(List<Zone> supplierZones,List<Code> codesToBeDeleted, int supplierId, int priceListId)
       {
           IImportPriceListDataManager dataManager = SupPLDataManagerFactory.GetDataManager<IImportPriceListDataManager>();
           dataManager.InsertPriceListObject(supplierZones, codesToBeDeleted, supplierId,priceListId);
       }
    }

   public class PriceListFileSettings : Vanrise.Entities.VRFileExtendedSettings
   {
       public override Guid ConfigId
       {
           get { return new Guid("7042919C-A79D-4600-943F-A0BE8E3CC4F7"); }
       }

       public int PriceListId { get; set; }

       Vanrise.Security.Business.SecurityManager s_securityManager = new Vanrise.Security.Business.SecurityManager();
       public override bool DoesUserHaveViewAccess(Vanrise.Entities.IVRFileDoesUserHaveViewAccessContext context)
       {
           return s_securityManager.HasPermissionToActions("WhS_BE/SupplierPricelist/GetFilteredSupplierPricelist", context.UserId);
       }
   }

}
