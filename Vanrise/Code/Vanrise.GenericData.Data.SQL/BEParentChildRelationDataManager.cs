using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class BEParentChildRelationDataManager : BaseSQLDataManager, IBEParentChildRelationDataManager
    {
        #region ctor/Local Variables
        public BEParentChildRelationDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }
        #endregion

        #region Public Methods

        public List<BEParentChildRelation> GetBEParentChildRelationes()
        {
            return GetItemsSP("[genericdata].[sp_BEParentChildRelation_GetAll]", BEParentChildRelationMapper);
        }

        public bool AreBEParentChildRelationUpdated(Guid beParentChildRelationDefinitionId, ref object updateHandle)
        {
            return base.IsDataUpdated("[genericdata].[BEParentChildRelation]", "RelationDefinitionId", beParentChildRelationDefinitionId, ref updateHandle);
        }

        public bool Insert(BEParentChildRelation beParentChildRelationItem, out long insertedId)
        {
            object beParentChildRelationID;

            int affectedRecords = ExecuteNonQuerySP("[genericdata].[sp_BEParentChildRelation_Insert]", out beParentChildRelationID, beParentChildRelationItem.RelationDefinitionId, beParentChildRelationItem.ParentBEId,
                beParentChildRelationItem.ChildBEId, beParentChildRelationItem.BED, beParentChildRelationItem.EED);

            insertedId = (affectedRecords > 0) ? (int)beParentChildRelationID : -1;
            return (affectedRecords > 0);
        }

        public bool Update(BEParentChildRelation BEParentChildRelationItem)
        {
            int affectedRecords = ExecuteNonQuerySP("[genericdata].[sp_BEParentChildRelation_Update]", BEParentChildRelationItem.BEParentChildRelationId, BEParentChildRelationItem.RelationDefinitionId, BEParentChildRelationItem.ParentBEId,
                BEParentChildRelationItem.ChildBEId, BEParentChildRelationItem.BED, BEParentChildRelationItem.EED);
            return (affectedRecords > 0);
        }

        #endregion

        #region Mappers

        BEParentChildRelation BEParentChildRelationMapper(IDataReader reader)
        {
            BEParentChildRelation beParentChildRelation = new BEParentChildRelation
            {
                BEParentChildRelationId = (long)reader["ID"],
                RelationDefinitionId = (Guid)reader["RelationDefinitionId"],
                ParentBEId = reader["ParentBEId"] as string,
                ChildBEId = reader["ChildBEId"] as string,
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
            return beParentChildRelation;
        }

        #endregion
    }
}
