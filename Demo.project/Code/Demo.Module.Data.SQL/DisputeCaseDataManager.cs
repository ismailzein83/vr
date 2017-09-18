using Demo.Module.Entities.DisputeCase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class DisputeCaseDataManager : BaseSQLDataManager, IDisputeCaseDataManager
    {
        public DisputeCaseDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }
        
        public List<DisputeCase> GetDisputeCases()
        {
            return GetItemsSP("[dbo].[sp_DisputeCase_GetAll]", DisputeCaseMapper);
        }
      
        public bool AreDisputeCasesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[DisputeCase]", ref updateHandle);
        }
        
        DisputeCase DisputeCaseMapper(IDataReader reader)
        {
            DisputeCase disputeCase = new DisputeCase();
            disputeCase.DisputeCaseId = GetReaderValue<int>(reader, "ID");
            disputeCase.CaseNumber = GetReaderValue<string>(reader, "CaseNumber");
            disputeCase.PartnerName = GetReaderValue<string>(reader, "PartnerName");
            disputeCase.PartnerType = GetReaderValue<string>(reader, "PartnerType");
            disputeCase.InvoiceNo = GetReaderValue<string>(reader, "InvoiceNo");
            disputeCase.Status = GetReaderValue<string>(reader, "Status");
            disputeCase.StatusCode = GetReaderValue<int>(reader, "StatusCode");
            disputeCase.CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime");
            disputeCase.CreatedBy = GetReaderValue<string>(reader, "CreatedBy");
            return disputeCase;
        }
    }
}
