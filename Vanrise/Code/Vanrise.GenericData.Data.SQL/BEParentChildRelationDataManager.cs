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

        public bool AreBEParentChildRelationUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[genericdata].[BEParentChildRelation]", ref updateHandle);
        }

        public bool Insert(BEParentChildRelation BEParentChildRelationItem, out long insertedId)
        {
            object beParentChildRelationID;

            int affectedRecords = ExecuteNonQuerySP("[genericdata].[sp_BEParentChildRelation_Insert]", out beParentChildRelationID, BEParentChildRelationItem.RelationDefinitionId, BEParentChildRelationItem.ParentBEId,
                BEParentChildRelationItem.ChildBEId, BEParentChildRelationItem.BED, BEParentChildRelationItem.EED);

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
                BEParentChildRelationId = (int)reader["ID"],
                RelationDefinitionId = (Guid)reader["RelationDefinitionId"],
                ParentBEId = reader["Name"] as string,
                ChildBEId = reader["Name"] as string,
                BED = (DateTime)reader["BED"],
                EED = (DateTime)reader["EED"],
            };
            return beParentChildRelation;
        }

        #endregion
    }
}
