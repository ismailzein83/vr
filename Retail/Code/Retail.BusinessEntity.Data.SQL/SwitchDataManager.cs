using Retail.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class SwitchDataManager :BaseSQLDataManager, ISwitchDataManager
    {
           
        #region ctor/Local Variables
        public SwitchDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "Retail_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods

     
        #endregion

        #region Private Methods

        #endregion

        #region  Mappers
       

        #endregion
    }
}
