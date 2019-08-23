using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class BaseBusinessEntityManager
    {
        public abstract List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context);

        public abstract dynamic GetEntity(IBusinessEntityGetByIdContext context);

        public abstract dynamic GetEntityId(IBusinessEntityIdContext context);

        public abstract string GetEntityDescription(IBusinessEntityDescriptionContext context);

        public abstract dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context);

        public abstract bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime);

        public abstract dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context);

        public abstract IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context);

        public virtual void GetIdByDescription(IBusinessEntityGetIdByDescriptionContext context)
        {

        }

        public virtual bool IsStillAvailable(IBusinessEntityIsStillAvailableContext context)
        {
            return true;
        }

        public virtual List<BusinessEntityCompatibleFieldInfo> GetCompatibleFields(IBusinessEntityGetCompatibleFieldsContext context)
        {
            return null;
        }
        public virtual bool TryGetStyleDefinitionId(IBusinessEntityStyleDefinitionContext context)
        {
            return false;
        }

        public virtual bool CanGetDescriptionByIds(IBusinessEntityCanGetDescriptionByIdsContext context)
        {
            return false;
        }

        public virtual Dictionary<object, string> GetDescriptionByIds(IBusinessEntityGetDescriptionByIdsContext context)
        {
            throw new NotImplementedException();
        }

        public virtual bool HasThreeSixtyDegreeView(IBusinessEntityHasThreeSixtyDegreeViewContext context)
        {
            return false;
        }
    }

    public class BusinessEntityCompatibleFieldInfo
    {
        public string FieldName { get; set; }

        public string FieldTitle { get; set; }
    }
    public interface IBusinessEntityStyleDefinitionContext
    {
        object FieldValue { get; }
        Guid StyleDefinitionId { set; }
    }
    public class BusinessEntityStyleDefinitionContext : IBusinessEntityStyleDefinitionContext
    {
        public object FieldValue { get; set; }
        public Guid StyleDefinitionId { get; set; }

    }

    public interface IBusinessEntityCanGetDescriptionByIdsContext
    {
        Guid BusinessEntityDefinitionId { get; }
    }

    public class BusinessEntityCanGetDescriptionByIdsContext : IBusinessEntityCanGetDescriptionByIdsContext
    {
        public Guid BusinessEntityDefinitionId { get; set; }
    }

    public interface IBusinessEntityGetDescriptionByIdsContext
    {
        Guid BusinessEntityDefinitionId { get; }
        IEnumerable<object> Values { get; }
    }

    public class BusinessEntityGetDescriptionByIdsContext : IBusinessEntityGetDescriptionByIdsContext
    {
        public Guid BusinessEntityDefinitionId { get; set; }

        public IEnumerable<object> Values { get; set; }
    }

    public class GenericBusinessEntityGetDescriptionByIdsInput
    {
        public Guid GenericBusinessEntityDefinitionId { get; set; }

        public List<object> Values { get; set; }
    }

    public interface IBusinessEntityHasThreeSixtyDegreeViewContext
    {
        Guid BusinessEntityDefinitionId { get; }
    }

    public class BusinessEntityHasThreeSixtyDegreeViewContext:  IBusinessEntityHasThreeSixtyDegreeViewContext
    {

        public Guid BusinessEntityDefinitionId { get; set; }
    }
}