using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Data;
using Vanrise.Data.SQL;
using Retail.BusinessEntity.Entities;

namespace  Retail.BusinessEntity.Data.SQL
{
    public class PopDataManager : BaseSQLDataManager, IPopDataManager
    {

        #region ctor/Local Variables
        public PopDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {
        }
        #endregion

        #region Public Methods
        public bool Insert(Pop pop, out int insertedId)
        {

            object popId;

            int recordsEffected = ExecuteNonQuerySP("dbo.sp_Pop_Insert", out popId, pop.Name, pop.Description , pop.Quantity , pop.Location);
            insertedId = (int)popId;
            return (recordsEffected > 0);
        }
        public bool Update(Pop pop)
        {
            int recordsEffected = ExecuteNonQuerySP("dbo.sp_Pop_Update", pop.PopId, pop.Name, pop.Description ,pop.Quantity, pop.Location);
            return (recordsEffected > 0);
        }
        public List<Pop> GetAllPops()
        {
            return GetItemsSP("dbo.sp_Pop_GetAll", PopMapper);
        }
        public bool ArePopsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("dbo.Pop", ref updateHandle);
        }
        #endregion


        #region  Mappers
        private Pop PopMapper(IDataReader reader)
        {
            Pop pop = new Pop
            {
                PopId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Description = reader["Description"] as string ,
                Quantity = GetReaderValue<int>(reader, "Quantity"),
                Location = reader["Location"] as string
            };
            return pop;
        }
        #endregion

    }
}
