using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;

namespace TOne.Web.Online.Controllers
{
    public class SupplierPriceListController : ApiController
    {
        [HttpPost]
        public bool UploadPriceList(SupplierPriceListUserInput userInput)
        {
            bool success = true;

            return success;
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