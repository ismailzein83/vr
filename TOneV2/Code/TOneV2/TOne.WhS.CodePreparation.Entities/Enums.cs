using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities
{
    public enum ImportType : short
    {
        New = 0,
        Delete = 1,
        Replace = -1
    }

    public enum CodePreparationStatus
    {
        Draft = 0,
        Completed = 1,
        Canceled = 2
    }

    public enum CodePreparationOutputResult
    {

        Inserted = 0,
        Failed = 1,
        Existing = 2,

    }


    public enum ValidationOutput
    {
        Success = 0,
        ValidationError =1,
        Failed=2
    }

}
