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
    public class PageDefinitionDataManager : BaseSQLDataManager, IPageDefinitionDataManager
    {
        public PageDefinitionDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<PageDefinition> GetPageDefinitions()
        {
            return GetItemsSP("[dbo].[sp_PageDefinition_GetAll]", PageDefinitionMapper);
        }

        public bool Insert(PageDefinition pageDefinition, out int insertedId)
        {
            object id;

            string serializedPageDefinitionDetails = null;

            if (pageDefinition.Details != null)
                serializedPageDefinitionDetails = Vanrise.Common.Serializer.Serialize(pageDefinition.Details);

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_PageDefinition_Insert]", out id, pageDefinition.Name, serializedPageDefinitionDetails);
            insertedId = Convert.ToInt32(id);

            return (nbOfRecordsAffected > 0);


        }

        public bool Update(PageDefinition pageDefinition)
        {
            string serializedPageDefinitionDetails = null;


            if (pageDefinition.Details != null)
                serializedPageDefinitionDetails = Vanrise.Common.Serializer.Serialize(pageDefinition.Details);

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_PageDefinition_Update]", pageDefinition.PageDefinitionId, pageDefinition.Name, serializedPageDefinitionDetails);

            return (nbOfRecordsAffected > 0);

        }

        public bool ArePageDefinitionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[PageDefinition]", ref updateHandle);

        }

        PageDefinition PageDefinitionMapper(IDataReader reader)
        {
            return new PageDefinition
               {
                   PageDefinitionId = GetReaderValue<int>(reader, "ID"),
                   Name = GetReaderValue<string>(reader, "Name"),
                   Details = Vanrise.Common.Serializer.Deserialize<Details>(reader["Details"] as string),
               };
        }


    }
}