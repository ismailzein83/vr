using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Business
{
    public class ComplaintManager
    {

        #region Public Methods
        public List<Complaint> GetComplaints()
        {
            IComplaintDataManager complaintDataManager = DemoModuleFactory.GetDataManager<IComplaintDataManager>();
            return complaintDataManager.GetComplaints();
        }
        #endregion
    }
}
