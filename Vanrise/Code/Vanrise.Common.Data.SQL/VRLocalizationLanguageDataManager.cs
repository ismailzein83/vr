using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Common.Data.SQL
{
    public class VRLocalizationLanguageDataManager:BaseSQLDataManager,IVRLocalizationLanguageDataManager
    {
        public VRLocalizationLanguageDataManager()
            : base(GetConnectionStringName("VRLocalizationDBDBConnStringKey", "VRLocalizationDBDBConnString"))
        {

        }
    }
}
