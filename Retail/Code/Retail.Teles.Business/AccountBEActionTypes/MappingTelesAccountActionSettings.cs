using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business.AccountBEActionTypes
{
    public class MappingTelesAccountActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("09A1029D-AFC0-48E4-B1B3-FD951462E267"); }
        }

        public override string ClientActionName
        {
            get { return " MappingTelesAccount"; }
        }
        public long DomainId { get; set; }
        public int SwitchId { get; set; }
    }
}
