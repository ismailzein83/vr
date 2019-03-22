using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using BPMExtended.Main.SOMAPI;
using SOM.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class BusinessEntityManager
    {
        //public UserConnection BPM_UserConnection
        //{
        //    get
        //    {
        //        return (UserConnection)HttpContext.Current.Session["UserConnection"];
        //    }
        //}    
        public Array GetLineOfBusniessArray()
        {
            var lineOfBusinessArray = Enum.GetNames(typeof(LineOfBusiness));
            return lineOfBusinessArray;
        }

    }
}
