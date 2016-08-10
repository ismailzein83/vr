using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRMailMessageTemplateDataManager : IDataManager
    {
        List<VRMailMessageTemplate> GetMailMessageTemplates();

        bool AreMailMessageTemplateUpdated(ref object updateHandle);

        bool Insert(VRMailMessageTemplate vrMailMessageTemplateItem);

        bool Update(VRMailMessageTemplate vrMailMessageTemplateItem);
    }
}
