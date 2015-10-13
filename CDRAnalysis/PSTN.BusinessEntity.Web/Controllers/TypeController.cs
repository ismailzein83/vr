using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Web.Controllers
{
    public class TypeController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<Type> GetTypes()
        {
            TypeManager manager = new TypeManager();
            return manager.GetTypes();
        }

        [HttpPost]
        public object GetFilteredTypes(Vanrise.Entities.DataRetrievalInput<TypeQuery> input)
        {
            TypeManager manager = new TypeManager();
            return GetWebResponse(input, manager.GetFilteredTypes(input));
        }

        [HttpGet]
        public Type GetTypeById(int switchTypeId)
        {
            TypeManager manager = new TypeManager();
            return manager.GetTypeById(switchTypeId);
        }

        [HttpPost]
        public InsertOperationOutput<Type> AddType(Type switchTypeObj)
        {
            TypeManager manager = new TypeManager();
            return manager.AddType(switchTypeObj);
        }

        [HttpPost]
        public UpdateOperationOutput<Type> UpdateType(Type switchTypeObj)
        {
            TypeManager manager = new TypeManager();
            return manager.UpdateType(switchTypeObj);
        }

        [HttpGet]
        public DeleteOperationOutput<object> DeleteType(int switchTypeId)
        {
            TypeManager manager = new TypeManager();
            return manager.DeleteType(switchTypeId);
        }
    }
}