using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data.SQL
{
    public class QueueInstanceDataManager : BaseSQLDataManager, IQueueInstanceDataManager
    {

        public QueueInstanceDataManager()
            : base(GetConnectionStringName("QueueDBConnStringKey", "QueueDBConnString"))
        {
        }

        public List<StageName> GetStageNames()
        {
            return GetItemsSP("queue.[sp_QueueInstance_GetAllStageNames]", StageNamesMapper);
        }


        public List<QueueInstance> GetAll()
        {
            throw new NotImplementedException();
        }

        #region Mappers
        private StageName StageNamesMapper(IDataReader reader)
        {
            return new StageName
            {
                Name = reader["StageName"] as string
                

            };
            
          } 
      #endregion






       
    }



}
