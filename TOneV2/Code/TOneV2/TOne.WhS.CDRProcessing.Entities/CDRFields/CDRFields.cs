using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
    public enum FieldType {Text=0,Number=1,DateTime=2,Boolean=3,Choices=4 }
    public class CDRFields : GenericConfiguration
    {
        public List<CDRField> Fields { get; set; }
        public override string OwnerKey
        {
            get { return null; }
        }
    }
    public class CDRField
    {
        public string FieldName { get; set; }
        public FieldType Type { get; set; }

    }
    public class CDRFieldDetail
    {

        public CDRField Entity { get; set; }
        public string TypeDescription { get; set; }

    }

}
