using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRCommentDataManager:IDataManager
    {
        VRComment GetVRCommentById(long vRCommentId);
        IEnumerable<VRComment> GetFilteredVRComments(DataRetrievalInput<VRCommentQuery> input);
        bool Insert(VRComment vrComment, out long reportId);

    }
}
