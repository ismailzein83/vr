using Demo.BestPractices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BestPractices.Business
{
    public class ChildShapeDescriptionContext : IChildShapeDescriptionContext
    {
        public Child Child { get; set; }
    }
}
