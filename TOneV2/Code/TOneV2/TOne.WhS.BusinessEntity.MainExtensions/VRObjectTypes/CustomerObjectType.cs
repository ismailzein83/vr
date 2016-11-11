using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class CustomerObjectType : VRObjectType
    {
        public override Guid ConfigId
        {
            get { return new Guid("48E39E5B-58A2-4799-89B3-F54ED3C48807"); } 
        }
    }
}
