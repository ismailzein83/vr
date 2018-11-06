using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public abstract class Setting
    {
        public Guid ConfigId { get; set; }
        public abstract string GetDescription();
        
    }

    public class MediumBranch : Setting
    {
        public int Size { get; set; }
        public string Location { get; set; }
        public int Employees { get; set; }

        public Dimensions Dimensions { get; set; }
        public override string GetDescription()
        {
            return ("Size: " + Size + " Location: " + Location + " Employees: " + Employees + (Dimensions != null ? Dimensions.GetDescription() : null));
        }
    }

    public class SmallBranch : Setting
    {
        public int Size { get; set; }
        public string Location { get; set; }
        public Dimensions Dimensions { get; set; }
        public override string GetDescription()
        {
            return ("Size: " + Size + " Location: " + Location + (Dimensions != null ? Dimensions.GetDescription() : null));

        }

    }
}
