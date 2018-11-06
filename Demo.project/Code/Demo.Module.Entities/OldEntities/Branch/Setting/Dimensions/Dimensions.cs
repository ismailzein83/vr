using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public abstract class Dimensions
    {
        public abstract Guid ConfigId { get; }
        public abstract string GetDescription();
    }

    public class Standard : Dimensions
    {
        public int Width { get; set; }
        public int Length { get; set; }
        
        public override string GetDescription()
        {
            return (" Width: "+Width+"\n Length: "+Length);

        }


        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
    }
    public class Advanced : Dimensions
    {
        public int Width { get; set; }
        public int Length { get; set; }
        public int Height { get; set; }
        public List<GridItem> GridItems { get; set; }
        public override string GetDescription()
        {
            return (" Width: " + Width + " Length: " + Length+" Height: "+ Height);

        }

        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
    }
    public class GridItem
    {
        public Guid Id { get; set;}
        public string Name{ get; set; }
    }
}
