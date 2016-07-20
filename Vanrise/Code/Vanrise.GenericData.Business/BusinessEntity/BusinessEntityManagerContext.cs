using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class BusinessEntityGetAllContext : BusinessEntityManagerContext, IBusinessEntityGetAllContext
    {
        public BusinessEntityGetAllContext(int businessEntityDefinitionId)
            : base(businessEntityDefinitionId)
        {
        }
        public BusinessEntityGetAllContext(BusinessEntityDefinition businessEntityDefinition)
            : base(businessEntityDefinition)
        {
        }
    }

    public class BusinessEntityIsCacheExpiredContext : BusinessEntityManagerContext, IBusinessEntityIsCacheExpiredContext
    {
        public BusinessEntityIsCacheExpiredContext(int businessEntityDefinitionId)
            : base(businessEntityDefinitionId)
        {
        }
        public BusinessEntityIsCacheExpiredContext(BusinessEntityDefinition businessEntityDefinition)
            : base(businessEntityDefinition)
        {
        }
    }

    public class BusinessEntityGetByIdContext : BusinessEntityManagerContext, IBusinessEntityGetByIdContext
    {
        public BusinessEntityGetByIdContext(int businessEntityDefinitionId)
            : base(businessEntityDefinitionId)
        {
        }
        public BusinessEntityGetByIdContext(BusinessEntityDefinition businessEntityDefinition)
            : base(businessEntityDefinition)
        {
        }

        public dynamic EntityId
        {
            get;
            set;
        }
    }

    public class BusinessEntityMapToInfoContext : BusinessEntityManagerContext, IBusinessEntityMapToInfoContext
    {
        public BusinessEntityMapToInfoContext(int businessEntityDefinitionId)
            : base(businessEntityDefinitionId)
        {
        }
        public BusinessEntityMapToInfoContext(BusinessEntityDefinition businessEntityDefinition)
            : base(businessEntityDefinition)
        {
        }

        public string InfoType { get; set; }

        public dynamic Entity
        {
            get;
            set;
        }
    }

    public class BusinessEntityGetParentEntityIdContext : BusinessEntityManagerContext, IBusinessEntityGetParentEntityIdContext
    {
        int _parentEntityDefinitionId;
        dynamic _entityId;
        public BusinessEntityGetParentEntityIdContext(int businessEntityDefinitionId, int parentEntityDefinitionId, dynamic entityId)
            : base(businessEntityDefinitionId)
        {
            _parentEntityDefinitionId = parentEntityDefinitionId;
            _entityId = entityId;
        }

        public int ParentEntityDefinitionId
        {
            get { return _parentEntityDefinitionId; }
        }

        BusinessEntityDefinition _parentEntityDefinition;
        public BusinessEntityDefinition ParentEntityDefinition
        {
            get
            {
                if (_parentEntityDefinition == null)
                {
                    _parentEntityDefinition = (new BusinessEntityDefinitionManager()).GetBusinessEntityDefinition(_parentEntityDefinitionId);
                    if (_parentEntityDefinition == null)
                        throw new NullReferenceException(String.Format("_parentEntityDefinition '{0}'", _parentEntityDefinitionId));
                }
                return _parentEntityDefinition;
            }
        }

        public dynamic EntityId
        {
            get
            {
                return _entityId;
            }
        }

        dynamic _entity;
        public dynamic Entity
        {
            get
            {
                if (_entity == null)
                {
                    BusinessEntityManager beManager = new BusinessEntityManager();
                    _entity = beManager.GetEntity(base.EntityDefinitionId, this._entityId);
                    if (_entity == null)
                        throw new NullReferenceException(string.Format("_entity. definitionId '{0}'. entityId '{1}'", base.EntityDefinitionId, this._entityId));
                }
                return _entity;
            }
        }
    }

    public class BusinessEntityGetIdsByParentEntityIdContext : BusinessEntityManagerContext, IBusinessEntityGetIdsByParentEntityIdContext
    {
        int _parentEntityDefinitionId;
        dynamic _parentEntityId;
        public BusinessEntityGetIdsByParentEntityIdContext(int businessEntityDefinitionId, int parentEntityDefinitionId, dynamic parentEntityId)
            : base(businessEntityDefinitionId)
        {
            _parentEntityDefinitionId = parentEntityDefinitionId;
            _parentEntityId = parentEntityId;
        }

        public int ParentEntityDefinitionId
        {
            get { return _parentEntityDefinitionId; }
        }

        BusinessEntityDefinition _parentEntityDefinition;
        public BusinessEntityDefinition ParentEntityDefinition
        {
            get
            {
                if (_parentEntityDefinition == null)
                {
                    _parentEntityDefinition = (new BusinessEntityDefinitionManager()).GetBusinessEntityDefinition(_parentEntityDefinitionId);
                    if (_parentEntityDefinition == null)
                        throw new NullReferenceException(String.Format("_parentEntityDefinition '{0}'", _parentEntityDefinitionId));
                }
                return _parentEntityDefinition;
            }
        }

        public dynamic ParentEntityId
        {
            get
            {
                return _parentEntityId;
            }
        }

        dynamic _parentEntity;
        public dynamic ParentEntity
        {
            get
            {
                if (_parentEntity == null)
                {
                    BusinessEntityManager beManager = new BusinessEntityManager();
                    _parentEntity = beManager.GetEntity(this.ParentEntityDefinitionId, this._parentEntityId);
                    if (_parentEntity == null)
                        throw new NullReferenceException(string.Format("_parentEntity. definitionId '{0}'. entityId '{1}'", this.ParentEntityDefinitionId, this._parentEntityId));
                }
                return _parentEntity;
            }
        }
    }

    public abstract class BusinessEntityManagerContext
    {
        int _businessEntityDefinitionId;
        BusinessEntityDefinition _businessEntityDefinition;

        public BusinessEntityManagerContext(int businessEntityDefinitionId)
        {
            _businessEntityDefinitionId = businessEntityDefinitionId;
        }

        public BusinessEntityManagerContext(BusinessEntityDefinition businessEntityDefinition)
        {
            if (businessEntityDefinition == null)
                throw new ArgumentNullException("businessEntityDefinition");
            _businessEntityDefinition = businessEntityDefinition;
            _businessEntityDefinitionId = businessEntityDefinition.BusinessEntityDefinitionId;
        }

        public BusinessEntityDefinition EntityDefinition
        {
            get
            {
                if(_businessEntityDefinition == null)
                {
                    _businessEntityDefinition = (new BusinessEntityDefinitionManager()).GetBusinessEntityDefinition(_businessEntityDefinitionId);
                    if (_businessEntityDefinition == null)
                        throw new NullReferenceException(String.Format("_businessEntityDefinition '{0}'", _businessEntityDefinitionId));
                }
                return _businessEntityDefinition;
            }
        }

        public int EntityDefinitionId
        {
            get { return _businessEntityDefinitionId; }
        }
    }
}
