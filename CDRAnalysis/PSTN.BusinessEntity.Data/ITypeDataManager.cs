using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface ITypeDataManager : IDataManager
    {
        List<Type> GetTypes();

        Vanrise.Entities.BigResult<Type> GetFilteredTypes(Vanrise.Entities.DataRetrievalInput<TypeQuery> input);

        Type GetTypeById(int switchTypeId);

        bool AddType(Type switchTypeObj, out int insertedId);

        bool UpdateType(Type switchTypeObj);

        bool DeleteType(int switchTypeId);
    }
}
