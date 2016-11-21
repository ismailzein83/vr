using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.NumberingPlan.Entities
{
    public class AddedZone : IZone , IRuleTarget
    {
        public long ZoneId { get; set; }

        public int CountryId { get; set; }

        public string Name { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        List<AddedCode> _addedCodes = new List<AddedCode>();
        public List<AddedCode> AddedCodes
        {
            get
            {
                return _addedCodes;
            }
        }

        public object Key
        {
            get { return this.Name; }
        }

        public string TargetType
        {
            get { return "Zone"; }
        }
    }
}
