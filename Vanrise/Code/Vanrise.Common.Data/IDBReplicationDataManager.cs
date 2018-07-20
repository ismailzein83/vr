using System;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IDBReplicationDataManager : IDataManager
    {
        void Initialise(IDBReplicationInitializeContext context);

        void MigrateData(IDBReplicationMigrateDataContext context);

        void Finalize(IDBReplicationFinalizeContext context);
    }
}
