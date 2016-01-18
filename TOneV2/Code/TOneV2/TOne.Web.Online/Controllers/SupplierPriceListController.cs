using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;

namespace TOne.Web.Online.Controllers
{
    public class SupplierPriceListController : ApiController
    {
        [HttpPost]
        public bool UploadPriceList(SupplierPriceListUserInput userInput)
        {
            //return true;
            int insertedId = 0;
            SupplierPriceListManager manager = new SupplierPriceListManager();
            return manager.SavePriceList(0, userInput.EffectiveOnDateTime, "C159", userInput.PriceListType, "portal@vanrise.com", userInput.ContentFile, userInput.FileName, out insertedId);
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