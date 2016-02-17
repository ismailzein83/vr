using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace Demo.Module.Data
{
    public interface IOperatorDeclaredInformationDataManager:IDataManager
    {
        List<OperatorDeclaredInformation> GetOperatorDeclaredInformations();
        bool Insert(OperatorDeclaredInformation info, out int infoId);
        bool Update(OperatorDeclaredInformation info);
        bool AreOperatorDeclaredInformationsUpdated(ref object updateHandle);
    }
}
