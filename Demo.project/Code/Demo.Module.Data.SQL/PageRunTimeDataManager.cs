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
    public class PageRunTimeDataManager : BaseSQLDataManager, IPageRunTimeDataManager
    {
        public PageRunTimeDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<PageRunTime> GetPageRunTimes()
        {
            return GetItemsSP("[dbo].[sp_PageRunTime_GetAll]", PageRunTimeMapper);
        }
       
        public bool Insert(PageRunTime pageRunTime, out int insertedId)
        {
            object id;

            string serializedPageRunTimeDetails = null;

            if (pageRunTime.Details != null)
                serializedPageRunTimeDetails = Vanrise.Common.Serializer.Serialize(pageRunTime.Details);

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_PageRunTime_Insert]", out id, pageRunTime.PageDefinitionId, serializedPageRunTimeDetails);
            insertedId = Convert.ToInt32(id);

            return (nbOfRecordsAffected > 0);


        }

        public bool Update(PageRunTime pageRunTime)
        {
            string serializedPageRunTimeDetails = null;


            if (pageRunTime.Details != null)
                serializedPageRunTimeDetails = Vanrise.Common.Serializer.Serialize(pageRunTime.Details);

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_PageRunTime_Update]", pageRunTime.PageRunTimeId, pageRunTime.PageDefinitionId, serializedPageRunTimeDetails);

            return (nbOfRecordsAffected > 0);

        }

        public bool ArePageRunTimesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[PageRunTime]", ref updateHandle);

        }

      
      
        PageRunTime PageRunTimeMapper(IDataReader reader)
        {
            return new PageRunTime
               {
                   PageDefinitionId = GetReaderValue<int>(reader, "PageDefinitionId"),
                   PageRunTimeId = GetReaderValue<int>(reader, "Id"),
                   Details = Vanrise.Common.Serializer.Deserialize<FieldDetails>(reader["Details"] as string),
               };
        }


    }
}