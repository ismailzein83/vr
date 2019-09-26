using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Player
{
    public class BeginnerPlayer : PlayerType
    {
        public override Guid ConfigID { get { return new Guid("E04BFED3-F61A-434E-B020-60A70DF408E7"); } }
    }
}