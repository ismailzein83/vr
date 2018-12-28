using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.RDBTests.Common
{
    public abstract class EntityMainOperationTester<T, Q>
    {
        T _entity;
        Q _rdbDataManager;
        Q _sqlDataManager;

        public EntityMainOperationTester(T entity, Q rdbDataManager, Q sqlDataManager)
        {
            _entity = entity;
            _rdbDataManager = rdbDataManager;
            _sqlDataManager = sqlDataManager;
        }

        public void ExecuteTest(bool testUniquenessOnAdd = true, bool testUniquenessOnUpdate = true, bool testDelete = false)
        {
            UTUtilities.TruncateTable(this.DBConnStringName, this.DBSchemaName, this.DBTableName);
            var entity = _entity;

            InsertEntity(entity);
            AssertEntitiesAreSimilar();

            ChangeEntityPropsForUpdate(entity);
            UpdateEntity(entity);
            AssertEntitiesAreSimilar();

            CopyAndTestInsertUpdate(entity);
            CopyAndTestInsertUpdate(entity);
            CopyAndTestInsertUpdate(entity);

            if (testUniquenessOnAdd)
            {
                T entityCopy = entity.VRDeepCopy();
                GenerateNewEntityId(entityCopy);
                InsertEntity(entityCopy);//Test Name Uniqueness on Insert
                AssertEntitiesAreSimilar();
            }

            if (testUniquenessOnUpdate)
            {
                T entityCopy = entity.VRDeepCopy();
                GenerateNewEntityId(entityCopy);
                SetUniqueValuesToUniqueProperties(entityCopy);
                InsertEntity(entityCopy);
                SetTargetUniquePropertiesEqualsToSource(entity, entityCopy);
                UpdateEntity(entityCopy);//Test Name Uniqueness on Update
                AssertEntitiesAreSimilar();
            }

            if (testDelete)
            {
                DeleteEntity(entity);
                AssertEntitiesAreSimilar();
            }
        }

        #region Private Methods

        private void AssertEntitiesAreSimilar()
        {
            var rdbAllEntities = GetAllEntities(_rdbDataManager);
            var sqlAllEntities = GetAllEntities(_sqlDataManager);
            UTUtilities.AssertObjectsAreSimilar(sqlAllEntities, rdbAllEntities);

            UTUtilities.AssertDBTablesAreSimilar(this.DBConnStringName, this.DBSchemaName, this.DBTableName);
        }

        private void CopyAndTestInsertUpdate(T entity)
        {
            T entityCopy = entity.VRDeepCopy();
            GenerateNewEntityId(entityCopy);
            SetUniqueValuesToUniqueProperties(entityCopy);
            InsertEntity(entityCopy);
            AssertEntitiesAreSimilar();

            ChangeEntityPropsForUpdate(entityCopy);
            UpdateEntity(entityCopy);
            AssertEntitiesAreSimilar();
        }

        private void InsertEntity(T entity)
        {            
            UTUtilities.AssertValuesAreEqual(InsertEntity(new EntityMainOperationTesterInsertEntityContext<T, Q>(entity, _sqlDataManager)), InsertEntity(new EntityMainOperationTesterInsertEntityContext<T, Q>(entity, _rdbDataManager)));
        }

        private void UpdateEntity(T entity)
        {
            UTUtilities.AssertValuesAreEqual(UpdateEntity(new EntityMainOperationTesterUpdateEntityContext<T, Q>(entity, _sqlDataManager)), UpdateEntity(new EntityMainOperationTesterUpdateEntityContext<T, Q>(entity, _rdbDataManager)));
        }

        private void GenerateNewEntityId(T entityCopy)
        {
            GenerateNewEntityId(new EntityMainOperationTesterGenerateNewEntityIdContext<T>(entityCopy));
        }

        private IEnumerable<T> GetAllEntities(Q dataManager)
        {
            return GetAllEntities(new EntityMainOperationTesterGetAllEntitiesContext<Q>(dataManager));
        }

        private void ChangeEntityPropsForUpdate(T entity)
        {
            ChangeEntityPropsForUpdate(new EntityMainOperationTesterChangeEntityPropsForUpdateContext<T>(entity));
        }

        private void DeleteEntity(T entity)
        {
            DeleteEntity(new EntityMainOperationTesterDeleteEntityContext<T, Q>(entity, _sqlDataManager));
            DeleteEntity(new EntityMainOperationTesterDeleteEntityContext<T, Q>(entity, _rdbDataManager));
        }

        private void SetUniqueValuesToUniqueProperties(T entity)
        {
            SetUniqueValuesToUniqueProperties(new EntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<T>(entity));
        }

        private void SetTargetUniquePropertiesEqualsToSource(T sourceEntity, T targetEntity)
        {
            SetTargetUniquePropertiesEqualsToSource(new EntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<T>(sourceEntity, targetEntity));
        }

        #endregion

        #region Abstract Methods

        public abstract string DBConnStringName { get; }

        public abstract string DBSchemaName { get; }

        public abstract string DBTableName { get; }

        public abstract bool InsertEntity(IEntityMainOperationTesterInsertEntityContext<T, Q> context);

        public abstract bool UpdateEntity(IEntityMainOperationTesterUpdateEntityContext<T, Q> context);

        public abstract void DeleteEntity(IEntityMainOperationTesterDeleteEntityContext<T, Q> context);

        public abstract IEnumerable<T> GetAllEntities(IEntityMainOperationTesterGetAllEntitiesContext<Q> context);
        
        public abstract void GenerateNewEntityId(IEntityMainOperationTesterGenerateNewEntityIdContext<T> context);
        
        public abstract void ChangeEntityPropsForUpdate(IEntityMainOperationTesterChangeEntityPropsForUpdateContext<T> context);
        
        public abstract void SetUniqueValuesToUniqueProperties(IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<T> context);

        public abstract void SetTargetUniquePropertiesEqualsToSource(IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<T> context);

        #endregion
    }

    public interface IEntityMainOperationTesterEntityContext<T>
    {
        T Entity { get; }
    }

    public class EntityMainOperationTesterEntityContext<T> : IEntityMainOperationTesterEntityContext<T>
    {
        public EntityMainOperationTesterEntityContext(T entity)
        {
            this.Entity = entity;
        }

        public T Entity { get; private set; }
    }

    public interface IEntityMainOperationTesterEntityDataManagerContext<T, Q> : IEntityMainOperationTesterEntityContext<T>
    {
        Q DataManager { get; }
    }

    public class EntityMainOperationTesterEntityDataManagerContext<T, Q> : EntityMainOperationTesterEntityContext<T>, IEntityMainOperationTesterEntityDataManagerContext<T, Q>
    {
        public EntityMainOperationTesterEntityDataManagerContext(T entity, Q dataManager)
            : base(entity)
        {
            this.DataManager = dataManager;
        }

        public Q DataManager { get; private set; }
    }

    public interface IEntityMainOperationTesterInsertEntityContext<T, Q> : IEntityMainOperationTesterEntityDataManagerContext<T, Q>
    {
    }

    public class EntityMainOperationTesterInsertEntityContext<T, Q> : EntityMainOperationTesterEntityDataManagerContext<T, Q>, IEntityMainOperationTesterInsertEntityContext<T, Q>
    {
        public EntityMainOperationTesterInsertEntityContext(T entity, Q dataManager)
            : base(entity, dataManager)
        {
        }
    }

    public interface IEntityMainOperationTesterUpdateEntityContext<T, Q> : IEntityMainOperationTesterEntityDataManagerContext<T, Q>
    {
    }

    public class EntityMainOperationTesterUpdateEntityContext<T, Q> : EntityMainOperationTesterEntityDataManagerContext<T, Q>, IEntityMainOperationTesterUpdateEntityContext<T, Q>
    {
        public EntityMainOperationTesterUpdateEntityContext(T entity, Q dataManager)
            : base(entity, dataManager)
        {
        }
    }

    public interface IEntityMainOperationTesterDeleteEntityContext<T, Q> : IEntityMainOperationTesterEntityDataManagerContext<T, Q>
    {
    }

    public class EntityMainOperationTesterDeleteEntityContext<T, Q> : EntityMainOperationTesterEntityDataManagerContext<T, Q>, IEntityMainOperationTesterDeleteEntityContext<T, Q>
    {
        public EntityMainOperationTesterDeleteEntityContext(T entity, Q dataManager)
            : base(entity, dataManager)
        {
        }
    }


    public interface IEntityMainOperationTesterGetAllEntitiesContext<Q>
    {
        Q DataManager { get; }
    }

    public class EntityMainOperationTesterGetAllEntitiesContext<Q> : IEntityMainOperationTesterGetAllEntitiesContext<Q>
    {
        public EntityMainOperationTesterGetAllEntitiesContext(Q dataManager)
        {
            this.DataManager = dataManager;
        }

        public Q DataManager { get; private set; }
    }

    public interface IEntityMainOperationTesterGetEntityIdContext<T> : IEntityMainOperationTesterEntityContext<T>
    {
    }

    public class EntityMainOperationTesterGetEntityIdContext<T> : EntityMainOperationTesterEntityContext<T>, IEntityMainOperationTesterGetEntityIdContext<T>
    {
        public EntityMainOperationTesterGetEntityIdContext(T entity)
            : base(entity)
        {
        }
    }


    public interface IEntityMainOperationTesterGenerateNewEntityIdContext<T> : IEntityMainOperationTesterEntityContext<T>
    {
    }

    public class EntityMainOperationTesterGenerateNewEntityIdContext<T> : EntityMainOperationTesterEntityContext<T>, IEntityMainOperationTesterGenerateNewEntityIdContext<T>
    {
        public EntityMainOperationTesterGenerateNewEntityIdContext(T entity)
            : base(entity)
        {
        }
    }

    public interface IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<T> : IEntityMainOperationTesterEntityContext<T>
    {

    }

    public class EntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<T> : EntityMainOperationTesterEntityContext<T>, IEntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext<T>
    {
        public EntityMainOperationTesterSetUniqueValuesToUniquePropertiesContext(T entity)
            : base(entity)
        {
        }
    }

    public interface IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<T>
    {
        T SourceEntity { get; }

        T TargetEntity { get; }
    }

    public class EntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<T> : IEntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext<T>
    {
        public EntityMainOperationTesterSetTargetUniquePropertiesEqualsToSourceContext(T sourceEntity, T targetEntity)
        {
            this.SourceEntity = sourceEntity;
            this.TargetEntity = targetEntity;
        }

        public T SourceEntity { get; private set; }

        public T TargetEntity { get; private set; }
    }

    public interface IEntityMainOperationTesterGetEntityNameContext<T> : IEntityMainOperationTesterEntityContext<T>
    {
    }

    public class EntityMainOperationTesterGetEntityNameContext<T> : EntityMainOperationTesterEntityContext<T>, IEntityMainOperationTesterGetEntityNameContext<T>
    {
        public EntityMainOperationTesterGetEntityNameContext(T entity)
            : base(entity)
        {
        }
    }

    public interface IEntityMainOperationTesterChangeEntityNameContext<T> : IEntityMainOperationTesterEntityContext<T>
    {
        string NewName { get; }
    }

    public class EntityMainOperationTesterChangeEntityNameContext<T> : EntityMainOperationTesterEntityContext<T>, IEntityMainOperationTesterChangeEntityNameContext<T>
    {
        public EntityMainOperationTesterChangeEntityNameContext(T entity, string newName)
            : base(entity)
        {
            this.NewName = newName;
        }

        public string NewName { get; private set; }
    }

    public interface IEntityMainOperationTesterChangeEntityPropsForUpdateContext<T> : IEntityMainOperationTesterEntityContext<T>
    {
    }

    public class EntityMainOperationTesterChangeEntityPropsForUpdateContext<T> : EntityMainOperationTesterEntityContext<T>, IEntityMainOperationTesterChangeEntityPropsForUpdateContext<T>
    {
        public EntityMainOperationTesterChangeEntityPropsForUpdateContext(T entity)
            : base(entity)
        {
        }
    }

    public interface IEntityMainOperationTesterClearUniquePropsForComparisonContext<T> : IEntityMainOperationTesterEntityContext<T>
    {
    }

    public class EntityMainOperationTesterClearUniquePropsForComparisonContext<T> : EntityMainOperationTesterEntityContext<T>, IEntityMainOperationTesterClearUniquePropsForComparisonContext<T>
    {
        public EntityMainOperationTesterClearUniquePropsForComparisonContext(T entity)
            : base(entity)
        {
        }
    }

}
