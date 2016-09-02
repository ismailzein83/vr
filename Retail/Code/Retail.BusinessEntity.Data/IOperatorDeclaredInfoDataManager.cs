using Retail.BusinessEntity.Entities;
using System.Collections.Generic;

namespace Retail.BusinessEntity.Data
{
    public interface IOperatorDeclaredInfoDataManager : IDataManager
    {
        List<OperatorDeclaredInfo> GetOperatorDeclaredInfos();
        bool AreOperatorDeclaredInfosUpdated(ref object updateHandle);

        bool Insert(OperatorDeclaredInfo operatorDeclaredInfo, out int insertedId);

        bool Update(OperatorDeclaredInfo operatorDeclaredInfo);

    }
}
