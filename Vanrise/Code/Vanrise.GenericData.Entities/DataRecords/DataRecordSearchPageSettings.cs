using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordSearchPageSettings : Vanrise.Security.Entities.ViewSettings
    {
        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/VR_GenericData/Views/DataRecordStorage/DataRecordStorageLog/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }

        public List<DRSearchPageStorageSource> Sources { get; set; }
    }

    public class DRSearchPageStorageSource
    {
        public string Title { get; set; }

        public Guid DataRecordTypeId { get; set; }

        public List<Guid> RecordStorageIds { get; set; }

        public List<DRSearchPageGridColumn> GridColumns { get; set; }
    }

    public class DRSearchPageGridColumn
    {
        public string FieldName { get; set; }

        public string FieldTitle { get; set; }
    }
}
