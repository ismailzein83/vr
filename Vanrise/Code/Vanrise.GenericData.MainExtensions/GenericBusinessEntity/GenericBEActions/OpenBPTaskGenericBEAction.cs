using System;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions
{
    public class OpenBPTaskGenericBEAction : GenericBEActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("157C3EAA-0EAC-40B5-AF4E-C6B9C3906347"); }
        }
        public override string ActionTypeName { get { return "OpenBPTaskGenericBEAction"; } }

        public override string ActionKind { get { return "OpenBPTask"; } }

        public string TaskIdFieldName { get; set; }
    }
}
