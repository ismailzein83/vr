using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRCommentDataManager : BaseSQLDataManager, IVRCommentDataManager
    {
        #region ctor/Local Variables
        public VRCommentDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion


        #region Public Methods

        public IEnumerable<VRComment> GetFilteredVRComments(DataRetrievalInput<VRCommentQuery> input)
        {
            return GetItemsSP("Common.sp_VRComment_GetFiltered", VRCommentMapper, input.Query.DefinitionId, input.Query.ObjectId);
        }

        public bool Insert(VRComment vRCommentItem, out long vRCommentId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("Common.sp_VRComment_Insert", out id, vRCommentItem.DefinitionId, vRCommentItem.ObjectId, vRCommentItem.Content, vRCommentItem.CreatedBy);

            bool result = (nbOfRecordsAffected > 0);
            if (result)
                vRCommentId = (long)id;
            else
                vRCommentId = 0;
            return result;
        }
        
        public VRComment GetVRCommentById(long vRCommentId)
        {
            return GetItemSP("Common.sp_VRComment_GetById", VRCommentMapper, vRCommentId); 
        }

        #endregion

        #region Mappers

        VRComment VRCommentMapper(IDataReader reader)
        {
            return new VRComment
            {
                VRCommentId = (long)reader["ID"],
                DefinitionId = (Guid)reader["DefinitionId"],
                ObjectId = reader["ObjectId"] as string,
                Content = reader["Content"] as string,
                CreatedBy = GetReaderValue<int>(reader, "CreatedBy"),
                CreatedTime = (DateTime)reader["CreatedTime"],
                LastModifiedTime = (DateTime)reader["LastModifiedTime"],
                LastModifiedBy = GetReaderValue<int>(reader, "LastModifiedBy")


            };
        }

        #endregion
    }

}