using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BaseProcessInputArgument
    {
        public virtual string ProcessName
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        public abstract string GetTitle();

        public virtual void MapExpressionValues(Dictionary<string, string> evaluatedExpressions)
        {

        }
    }
}
