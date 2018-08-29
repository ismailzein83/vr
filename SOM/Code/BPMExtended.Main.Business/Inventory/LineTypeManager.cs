using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.Business
{
    public class LineTypeManager
    {
        public List<LineType> GetLineTypes()
        {
            return new List<LineType>() { 
                                        new LineType { Id = new Guid("099885D2-D5E9-4D56-9722-5F0135932C6A"), Name = "PSTN" }, 
                                        new LineType { Id = new Guid("FDBEF1FF-16C5-4DE3-AC70-7ABE27903711"), Name = "ISDN" }, 
                                        new LineType { Id = new Guid("ABE03B5E-9BD8-4A2E-B061-0C33A7394B35"), Name = "DID" }, 
                                        new LineType { Id = new Guid("7EC011F2-76B5-4AB7-97F6-7DFFDFFFB55E"), Name = "WLL" }, 
                                        new LineType { Id = new Guid("BA8715C5-DAB5-4967-A711-A694F484024F"), Name = "POST" }, 
                                        new LineType { Id = new Guid("767CA27D-8278-45D5-BD3F-B95A0601FEF5"), Name = "Fiber" } };
        }
    }
}
