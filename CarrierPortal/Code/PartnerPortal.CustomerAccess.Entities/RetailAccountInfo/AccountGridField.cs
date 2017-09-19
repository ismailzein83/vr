using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace PartnerPortal.CustomerAccess.Entities
{
    public class AccountGridField
    {
        public string FieldName { get; set; }
        public string FieldTitle { get; set; }
        public GridColumnSettings ColumnSettings { get; set; }
    }
}
