using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class CreatePaymentPlanManager
    {
        #region User Connection
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        #endregion

        #region public
        public void PostCreatePaymentPlanRequestToOM(Guid requestId)
        {

            EntitySchemaQuery esq;
            IEntitySchemaQueryFilterItem esqFirstFilter;
            EntityCollection entities;
            string flags;
            string statusId;

            //Call Categories catalog and check the 'IsNormal' field if true => no need for attachments (optional), if false => attachment is required 
            esq = new EntitySchemaQuery(BPM_UserConnection.EntitySchemaManager, "StCreatePaymentPlan");
            esq.AddColumn("Id");
            esq.AddColumn("StFlags");

            esqFirstFilter = esq.CreateFilterWithParameters(FilterComparisonType.Equal, "Id", requestId);
            esq.Filters.Add(esqFirstFilter);

            entities = esq.GetEntityCollection(BPM_UserConnection);
            if (entities.Count > 0)
            {
                flags = (string)entities[0].GetColumnValue("StFlags");

                Flag flag = JsonConvert.DeserializeObject<Flag>(flags);

                if (flag.isApproval)
                {
                    statusId = "8057E9A4-24DE-484D-B202-0D189F5B7758";
                }
                else
                {
                    statusId = "87486F6C-FD2B-498D-8E4A-90935299058E";
                }

                //TODO: update status in 'request header' table
                UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
                var update = new Update(connection, "StRequestHeader").Set("StStatusId", Column.Parameter(statusId))
                    .Where("StRequestId").IsEqual(Column.Parameter(requestId));
                update.Execute();

            }
        }
        #endregion
    }
}
