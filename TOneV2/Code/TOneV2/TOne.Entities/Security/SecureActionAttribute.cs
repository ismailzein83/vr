using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SecureActionAttribute : Attribute
    {
        public SecureActionAttribute(string modulePath, string actionName)
        {
            _modulePath = modulePath;
            _actionName = actionName;
        }

        public SecureActionAttribute(string actionName)
            : this(null, actionName)
        {

        }

        private string _modulePath;

        public string ModulePath
        {
            get
            {
                return _modulePath;
            }
        }

        private string _actionName;

        public string ActionName
        {
            get { return _actionName; }
        }
    }
}
