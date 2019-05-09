using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
   public class AccountManagerDataRecordStoragePermanentFilter : DataRecordStoragePermanentFilterSettings
    {
        public override Guid ConfigId => new Guid("A6F4D0D4-3562-4151-8ED8-984CE7A83C20");
        public override void ConvertToRecordFilter()
        {
            throw new NotImplementedException();
        }
    }
}
