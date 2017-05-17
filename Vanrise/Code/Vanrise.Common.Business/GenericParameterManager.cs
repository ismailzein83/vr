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
        GeneralSettingsManager _generalSettingsManager;
        public static GenericParameterManager Current
        {
            get
            {
                return s_current;
            }
        }

        private GenericParameterManager()
        {
            _generalSettingsManager = new GeneralSettingsManager();
        }

        #endregion
        public int GetNormalPrecision()
        {
            return _generalSettingsManager.GetNormalPrecisionValue();
        }

        public int GetLongPrecision()
        {
            return _generalSettingsManager.GetLongPrecisionValue();
        }
    }
}
