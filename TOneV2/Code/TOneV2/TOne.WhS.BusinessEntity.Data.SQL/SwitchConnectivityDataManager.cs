﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SwitchConnectivityDataManager : BaseSQLDataManager, ISwitchConnectivityDataManager
    {
        #region Constructors

        public SwitchConnectivityDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneV2DBConnString"))
        {

        }

        #endregion
    }
}
