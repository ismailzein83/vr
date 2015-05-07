using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
     [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SecureControllerAttribute : Attribute
    {
        public SecureControllerAttribute(string modulePath)
        {
            _modulePath = modulePath;
        }

        private string _modulePath;

        public string ModulePath
        {
            get
            {
                return _modulePath;
            }
        }
    }
}
