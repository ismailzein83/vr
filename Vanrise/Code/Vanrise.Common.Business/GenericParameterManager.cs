using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Business
{
    public class GenericParameterManager
    {
        #region Singleton

        static GenericParameterManager s_current = new GenericParameterManager();

        public static GenericParameterManager Current
        {
            get
            {
                return s_current;
            }
        }

        private GenericParameterManager()
        {

        }

        #endregion
        public int GetNormalPrecision()
        {
            return 2;
        }

        public int GetLongPrecision()
        {
            return 4;
        }
    }
}
