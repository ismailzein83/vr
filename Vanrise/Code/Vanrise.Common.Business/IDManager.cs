using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;

namespace Vanrise.Common.Business
{
    public class IDManager
    { 
        #region Singleton

        static IDManager s_instance = new IDManager();
        public static IDManager Instance
        {
            get
            {
                return s_instance;
            }
        }

        private IDManager()
        {

        }
        
        #endregion
        IIDManagerDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IIDManagerDataManager>();

        public void ReserveIDRange(Type t, int nbOfIds, out long startingId)
        {
            _dataManager.ReserveIDRange(TypeManager.Instance.GetTypeId(t), nbOfIds, out startingId);
        }
    }
}
