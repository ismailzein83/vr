using QM.BusinessEntity.Business;
using QM.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using System.Web;
using System.IO;

namespace QM.BusinessEntity.Web.Controllers
{
   
    [RoutePrefix(Constants.ROUTE_PREFIX + "Supplier")]

    [JSONWithTypeAttribute]
    public class QMBE_SupplierController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSuppliers")]
        public object GetFilteredSuppliers(Vanrise.Entities.DataRetrievalInput<SupplierQuery> input)
        {
            SupplierManager manager = new SupplierManager();
            return GetWebResponse(input, manager.GetFilteredSuppliers(input));
        }

        [HttpGet]
        [Route("GetSupplier")]
        public Supplier GetSupplier(int supplierId)
        {
            SupplierManager manager = new SupplierManager();
            return manager.GetSupplier(supplierId);
        }

        [HttpGet]
        [Route("GetSuppliersInfo")]
        public IEnumerable<SupplierInfo> GetSuppliersInfo()
        {
            SupplierManager manager = new SupplierManager();
            return manager.GetSuppliersInfo();
        }

        [HttpPost]
        [Route("AddSupplier")]
        public InsertOperationOutput<SupplierDetail> AddSupplier(Supplier supplier)
        {
            SupplierManager manager = new SupplierManager();
            return manager.AddSupplier(supplier);
        }

        [HttpPost]
        [Route("UpdateSupplier")]
        public UpdateOperationOutput<SupplierDetail> UpdateSupplier(Supplier supplier)
        {
            SupplierManager manager = new SupplierManager();
            return manager.UpdateSupplier(supplier);
        }

        [HttpGet]
        [Route("GetSupplierSourceTemplates")]
        public IEnumerable<SourceSupplierReaderConfig> GetSupplierSourceTemplates()
        {
            SupplierManager manager = new SupplierManager();
            return manager.GetSupplierSourceTemplates();
        }


        [HttpGet]
        [Route("DownloadImportSupplierTemplate")]
        public object DownloadImportSupplierTemplate()
        {
            var templatePath = "~/Client/Modules/QM_BusinessEntity/Templates/ImportSupplierTemplate.xls";
            string physicalPath = HttpContext.Current.Server.MapPath(templatePath);
            byte[] fileInBytes = File.ReadAllBytes(physicalPath);

            MemoryStream memorystream = new MemoryStream();
            memorystream.Write(fileInBytes, 0, fileInBytes.Length);
            memorystream.Seek(0, SeekOrigin.Begin);

            return GetExcelResponse(memorystream, "Supplier Import Template.xls");
        }



        [HttpGet]
        [Route("UploadSuppliersList")]

        public string UploadSuppliersList(int fileId, bool allowUpdate)
        {
            SupplierManager manager = new SupplierManager();
            return manager.AddSuppliers(fileId, allowUpdate);
        }

    }
}