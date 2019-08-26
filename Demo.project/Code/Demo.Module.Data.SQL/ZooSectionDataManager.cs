using Demo.Module.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    class ZooSectionDataManager : BaseSQLDataManager, IZooSectionDataManager
    {
        //#region Properties/Ctor

        //public ZooSectionDataManager() :
        //    base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        //{
        //}

        //#endregion

        //#region Public Methods

        //public List<Entities.ZooSection> GetZooSections()
        //{
        //    return GetItemsSP("[dbo].[sp_ZooSection_GetAll]", ZooSectionMapper);
        //}

        //public bool Insert(ZooSection zooSection, out long insertedId)
        //{
        //    object id;
        //    int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_ZooSection_Insert]", out id , zooSection.Name, zooSection.ZooId, zooSection.Position, zooSection.Type);

        //    bool result = nbOfRecordsAffected > 0;

        //    if (result)
        //        insertedId = (long)id;
        //    else
        //        insertedId = 0;

        //    return result;
        //}

        //public bool Update(ZooSection zooSection)
        //{
        //    int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_ZooSection_Update]", zooSection.ZooSectionId, zooSection.Name, zooSection.ZooId, zooSection.Position, zooSection.Type);
        //    return nbOfRecordsAffected > 0;
        //}

        //#endregion

        //#region Mappers

        //public ZooSection ZooSectionMapper(IDataReader reader)
        //{
        //    return new ZooSection()
        //    {
        //        ZooSectionId = GetReaderValue<long>(reader, "ID"),
        //        Name = GetReaderValue<string>(reader, "Name"),
        //    };
        //}

        //#endregion
    }
}
