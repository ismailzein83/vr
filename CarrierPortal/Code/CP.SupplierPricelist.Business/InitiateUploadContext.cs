using CP.SupplierPricelist.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace CP.SupplierPricelist.Business
{
    public class InitiateUploadContext : IInitiateUploadContext
    {
        public int UserId { get; set; }
        public string PriceListType { get; set; }
        public VRFile File { get; set; }
        public DateTime EffectiveOnDateTime { get; set; }
    }
}
