using System;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.Web.Online.Controllers
{
    public class SupplierPriceListController : ApiController
    {
        [HttpPost]
        public int UploadPriceList(SupplierPriceListUserInput userInput)
        {
            //return true;
            int insertedId;
            SupplierPricelistsManager manager = new SupplierPricelistsManager();
            manager.SavePriceList(0, userInput.EffectiveOnDateTime, "C159", userInput.PriceListType, "portal@vanrise.com", userInput.ContentFile, userInput.FileName, out insertedId);
            return insertedId;
        }

        [HttpGet]
        public int GetResults(int queueId)
        {
            SupplierPricelistsManager manager = new SupplierPricelistsManager();
            return manager.GetQueueStatus(queueId);
        }

        [HttpGet]
        public UploadInfo GetUploadInfo(int queueId)
        {
            SupplierPricelistsManager manager = new SupplierPricelistsManager();
            return manager.GetUploadInfo(queueId);
        }

        [HttpGet]
        public string ping()
        {
            return "hello wolrd";
        }
        public class SupplierPriceListUserInput
        {
            public int UserId { get; set; }
            public string PriceListType { get; set; }
            public byte[] ContentFile { get; set; }
            public string FileName { get; set; }
            public DateTime EffectiveOnDateTime { get; set; }
        }
    }
}