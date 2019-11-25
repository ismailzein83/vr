using System;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions
{
    public class CompileGenericBEAction : GenericBEActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("637FD561-2DAA-4BC3-8DF6-C00245EF78EC"); }
        }
        public override string ActionTypeName { get { return "CompileGenericBEAction"; } }

        public override string ActionKind { get { return "Compile"; } }

    }
}
