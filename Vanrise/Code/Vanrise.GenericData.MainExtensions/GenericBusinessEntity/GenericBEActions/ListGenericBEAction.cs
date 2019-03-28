using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions
{
    public class ListGenericBEAction : GenericBEActionSettings
    {
        public override Guid ConfigId { get { return new Guid("1E1C7DD8-29BE-4614-8F06-C9DC4D0D3E3E"); } }
        public override string ActionKind { get { return "ListGenericBEAction"; } }
        public override string ActionTypeName { get { return "ListGenericBEAction"; } }
        public List<GenericBEAction> GenericBEActions { get; set; }
    }
}
