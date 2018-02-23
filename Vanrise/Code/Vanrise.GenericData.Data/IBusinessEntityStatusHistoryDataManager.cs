using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface IBusinessEntityStatusHistoryDataManager:IDataManager
    {
        bool Insert(Guid businessEntityDefinitionId, Object businessEntityId, string fieldName, Guid statusId, Guid? previousStatusId);

    }
}
