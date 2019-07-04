using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEActions
{
    public class DownloadFileGenericBEAction : GenericBEActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("8EF5476F-37EB-436F-8558-E3181AB78C82"); }
        }
        public override string ActionTypeName { get { return "DownloadFileGenericBEAction"; } }

        public override string ActionKind { get { return "DownloadFile"; } }
        public string FileIdFieldName { get; set; }
        public bool OpenNewWindow { get; set; }
    }
}
