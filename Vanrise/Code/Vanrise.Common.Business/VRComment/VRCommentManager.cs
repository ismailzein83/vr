using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common.Data;
using Vanrise.Entities;


namespace Vanrise.Common.Business
{
    public class VRCommentManager
    {
        Vanrise.Security.Business.UserManager _userManager = new Vanrise.Security.Business.UserManager();

        #region Public Methods
        public IDataRetrievalResult<VRCommentDetail> GetFilteredVRComments(DataRetrievalInput<VRCommentQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new VRCommentRequestHandler());
        }
       
               
        public InsertOperationOutput<VRCommentDetail> AddVRComment(VRComment vRComment)
        {
            IVRCommentDataManager vRCommentDataManager = CommonDataManagerFactory.GetDataManager<IVRCommentDataManager>();
            InsertOperationOutput<VRCommentDetail> insertOperationOutput = new InsertOperationOutput<VRCommentDetail>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long commentId = -1;
            vRComment.CreatedBy = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            bool insertActionSuccess = vRCommentDataManager.Insert(vRComment, out commentId);
            if (insertActionSuccess)
            {  
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                VRComment addedVRComment = this.GetVRCommentById(commentId);
                insertOperationOutput.InsertedObject = VRCommentDetailMapper(addedVRComment);
            }
           
            return insertOperationOutput;
        }

        public VRComment GetVRCommentById(long commentId)
        {
            IVRCommentDataManager vRCommentDataManager = CommonDataManagerFactory.GetDataManager<IVRCommentDataManager>();
            return vRCommentDataManager.GetVRCommentById(commentId);
        }
        #endregion

        #region Private Classes
        private class VRCommentRequestHandler : BigDataRequestHandler<VRCommentQuery, VRComment, VRCommentDetail>
        {
            public VRCommentRequestHandler()
            {

            }
            public override VRCommentDetail EntityDetailMapper(VRComment vRComment)
            {
                VRCommentManager vRCommentManager = new VRCommentManager();
                return vRCommentManager.VRCommentDetailMapper(vRComment);

            }
            public override IEnumerable<VRComment> RetrieveAllData(DataRetrievalInput<VRCommentQuery> input)
            {
                IVRCommentDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRCommentDataManager>();
                return _dataManager.GetFilteredVRComments(input);
            }

        }

    
        #endregion

        #region Mappers
        public VRCommentDetail VRCommentDetailMapper(VRComment vRComment)
        {            
            return new VRCommentDetail
            { VRCommentId=vRComment.VRCommentId,
                Content = vRComment.Content,
                CreatedByDescription = _userManager.GetUserName(vRComment.LastModifiedBy),
                CreatedBy = vRComment.CreatedBy,
                CreatedTime = vRComment.CreatedTime
            };
        }


        #endregion

    }
   
}
