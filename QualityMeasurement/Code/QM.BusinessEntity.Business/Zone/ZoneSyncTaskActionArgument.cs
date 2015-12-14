using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Business
{
    public class ZoneSyncTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public SourceZoneReader SourceZoneReader { get; set; }

        public Vanrise.Entities.SourceCountryReader SourceCountryReader { get; set; }
    }
}
