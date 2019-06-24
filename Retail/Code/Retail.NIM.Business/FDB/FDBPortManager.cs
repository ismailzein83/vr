using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class FDBPortManager
    {
        public static Guid s_beDefinitionId = new Guid("70b2ee0b-9508-4a77-82bc-b14702b99f27");

        #region Public Methods

        public int GetFreePortsNb(long fdbId)
        {
            return GetPortsNb(fdbId, PortStatus.Free);
        }

        public int GetUsedPortsNb(long fdbId)
        {
            return GetPortsNb(fdbId, PortStatus.Used);
        }

        public int GetPortsNb(long fdbId, Guid statusId)
        {
            List<FDBPort> fdpPorts = GetFDBPorts(fdbId);
            if (fdpPorts == null || fdpPorts.Count == 0)
                return 0;

            int freePortsNb = 0;

            foreach (var fdpPort in fdpPorts)
            {
                if (fdpPort.Status == statusId)
                    freePortsNb++;
            }

            return freePortsNb;
        }

        #endregion

        #region Private Methods

        private List<FDBPort> GetFDBPorts(long fdbId)
        {
            Dictionary<long, List<FDBPort>> fdbPortsByFDB = GetCachedFDBPortsByFDB();
            return fdbPortsByFDB.GetRecord(fdbId);
        }

        private Dictionary<long, List<FDBPort>> GetCachedFDBPortsByFDB()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedFDBPortsByFDB", s_beDefinitionId, () =>
            {
                List<FDBPort> fdbPorts = this.GetCachedFDBPorts();

                Dictionary<long, List<FDBPort>> fdbPortsByFDB = new Dictionary<long, List<FDBPort>>();

                foreach (var fdbPort in fdbPorts)
                {
                    List<FDBPort> currentFDBPorts = fdbPortsByFDB.GetOrCreateItem(fdbPort.FDB);
                    currentFDBPorts.Add(fdbPort);
                }

                return fdbPortsByFDB;
            });
        }

        private List<FDBPort> GetCachedFDBPorts()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedFDBPorts", s_beDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(s_beDefinitionId);

                List<FDBPort> results = new List<FDBPort>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        FDBPort fdb = new FDBPort()
                        {
                            Id = (long)genericBusinessEntity.FieldValues.GetRecord("Id"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Area = (long)genericBusinessEntity.FieldValues.GetRecord("Area"),
                            Site = (long)genericBusinessEntity.FieldValues.GetRecord("Site"),
                            FDB = (long)genericBusinessEntity.FieldValues.GetRecord("FDB"),
                            Status = (Guid)genericBusinessEntity.FieldValues.GetRecord("Status"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
                        };
                        results.Add(fdb);
                    }
                }

                return results;
            });
        }

        #endregion
    }
}
