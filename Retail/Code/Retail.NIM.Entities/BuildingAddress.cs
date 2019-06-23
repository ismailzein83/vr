using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public struct BuildingAddress
    {
        public long Street { get; set; }

        public string BuildingDetails { get; set; }

        public override int GetHashCode()
        {
            return this.Street.GetHashCode(); // + this.BuildingDetails.GetHashCode();
        }
    }
}