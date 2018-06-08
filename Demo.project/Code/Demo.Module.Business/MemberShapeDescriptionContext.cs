using Demo.Module.Entities.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Business
{
    class MemberShapeDescriptionContext : IMemberShapeDescriptionContext
    {
        public Member Member { get; set; }

    }
}
