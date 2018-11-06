using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class CustomerInfoDataManager : BaseSQLDataManager, ICustomerInfoDataManager
    {

        #region Constructors
        public CustomerInfoDataManager() :
            base(GetConnectionStringName("DemoProjectCallCenter_DBConnStringKey", "DemoProjectCallCenter_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public CustomerInfo GetCustomerInfo()
        {
            return GetItemSP("[CcEntities].[sp_CustomerInfo_GetCustomerInfo]", CustomerInfoMapper);
        }
        #endregion  


        #region Mappers
        CustomerInfo CustomerInfoMapper(IDataReader reader)
        {
            return new CustomerInfo
            {
                CustomerId = GetReaderValue<long>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                Gender = GetReaderValue<string>(reader, "Gender"),
                Age = GetReaderValue<string>(reader, "Age"),
                Address = GetReaderValue<string>(reader, "Address"),
                MobileNumber = GetReaderValue<string>(reader, "MobileNumber"),
                Photo = "data:image/jpg;base64," + GetReaderValue<string>(reader, "Photo"),
                Email = GetReaderValue<string>(reader, "Email")
            };
        }
        #endregion
    }
}
