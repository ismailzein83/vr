using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class Product
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "Retail_BusinessEntity_Product";
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("41767702-B520-4811-96BE-103F96B81177");
        public int ProductId { get; set; }

        public string Name { get; set; }

        public ProductSettings Settings { get; set; }
    }
}
