using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBECustomAction
{
    class GenericBEUploadCustomAction : GenericBECustomActionSettings
    {
        public override Guid ConfigId => new Guid ("{1F7BEC62-A10C-4F3E-AC71-BDB0A7B670FF}");

        public override string ActionTypeName => "UploadCustomAction";
    }
}
