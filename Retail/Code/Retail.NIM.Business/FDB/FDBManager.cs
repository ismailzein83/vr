using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class FDBManager
    {
        public static Guid s_beDefinitionId = new Guid("eadb13a2-20a2-4afa-85fc-3fa58a6d0b6d");

        #region Public Methods

        public GetFDBReservationInfoOutput GetFDBReservationInfo(GetFDBReservationInfoInput input)
        {
            input.ThrowIfNull("GetFDBReservationInfoInput");

            FDB fdb;

            switch (input.NumberType)
            {
                case NumberType.PhoneNumber:
                    fdb = GetFDBByNearestPhoneNumber(input.Number);
                    break;

                case NumberType.DP:
                    DP dp = new DPManager().GetDPByNumber(input.Number);
                    fdb = GetFDBByDP(dp);
                    break;

                case NumberType.FDB:
                    fdb = GetFDBByNumber(input.Number);
                    break;

                default:
                    throw new NotSupportedException($"NumberType {input.NumberType} not supported.");
            }

            if (fdb == null)
                return null;

            return new GetFDBReservationInfoOutput(fdb);
        }

        public ReserveFTTHPathOutput ReserveFTTHPath(ReserveFTTHPathInput input)
        {
            input.ThrowIfNull("ReserveFTTHPathInput");

            FreeFTTHPath freeFTTHPath = new FreeFTTHPathManager().GetFreeFTTHPath(input.FDBNumber);
            if (freeFTTHPath == null)
                return null;

            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();

            List<GenericBEIdentifier> genericBEPortIdentifierList = BuildGenericBEPortIdentifierList(freeFTTHPath);

            foreach (var genericBEPortIdentifier in genericBEPortIdentifierList)
            {
                UpdateGenericBEPortStatus(genericBEPortIdentifier, genericBusinessEntityManager);
            }

            return new ReserveFTTHPathOutput(freeFTTHPath);
        }

        #endregion

        #region Private Methods

        private void UpdateGenericBEPortStatus(GenericBEIdentifier genericBEPortIdentifier, GenericBusinessEntityManager genericBusinessEntityManager)
        {
            GenericBusinessEntityToUpdate genericBusinessEntityToUpdate = new GenericBusinessEntityToUpdate()
            {
                BusinessEntityDefinitionId = genericBEPortIdentifier.GenericBEDefinitionId,
                GenericBusinessEntityId = genericBEPortIdentifier.GenericBEId,
                FieldValues = new Dictionary<string, object>()
            };
            genericBusinessEntityToUpdate.FieldValues.Add("Status", PortStatus.TemporaryReserved);
            genericBusinessEntityManager.UpdateGenericBusinessEntity(genericBusinessEntityToUpdate, null);
        }

        private List<GenericBEIdentifier> BuildGenericBEPortIdentifierList(FreeFTTHPath freeFTTHPath)
        {
            List<GenericBEIdentifier> genericBEIdentifierList = new List<GenericBEIdentifier>();
            genericBEIdentifierList.Add(new GenericBEIdentifier(FDBPortManager.s_beDefinitionId, freeFTTHPath.FDBPort));
            genericBEIdentifierList.Add(new GenericBEIdentifier(SplitterOutPortManager.s_beDefinitionId, freeFTTHPath.SplitterOutPort));
            genericBEIdentifierList.Add(new GenericBEIdentifier(SplitterInPortManager.s_beDefinitionId, freeFTTHPath.SplitterInPort));
            genericBEIdentifierList.Add(new GenericBEIdentifier(OLTVerticalPortManager.s_beDefinitionId, freeFTTHPath.OLTVerticalPort));
            genericBEIdentifierList.Add(new GenericBEIdentifier(OLTHorizontalPortManager.s_beDefinitionId, freeFTTHPath.OLTHorizontalPort));
            genericBEIdentifierList.Add(new GenericBEIdentifier(IMSTIDManager.s_beDefinitionId, freeFTTHPath.TID));
            return genericBEIdentifierList;
        }

        private FDB GetFDBByNearestPhoneNumber(string nearestPhoneNumber)
        {
            if (string.IsNullOrEmpty(nearestPhoneNumber))
                return null;

            //try to get FDB from Fiber
            //Not Implemented Yet

            //try to get FDB from Copper
            CopperPath copperPath = new CopperPathManager().GetPathByFullPhoneNumber(nearestPhoneNumber);
            if (copperPath != null)
            {
                DP dp = new DPManager().GetDPById(copperPath.DP);
                FDB fdb = GetFDBByDP(dp);
                if (fdb != null)
                    return fdb;
            }

            return null;
        }

        private FDB GetFDBByNumber(string fdbNumber)
        {
            Dictionary<string, FDB> fdbsByNumber = this.GetCachedFDBsByNumber();
            return fdbsByNumber.GetRecord(fdbNumber);
        }

        private FDB GetFDBByDP(DP dp)
        {
            dp.ThrowIfNull("DP", dp.Id);

            BuildingAddress dpBuildingAddress = new BuildingAddress() { Street = dp.Street, BuildingDetails = dp.BuildingDetails };

            Dictionary<BuildingAddress, FDB> fdbByBuildingAddress = this.GetCachedFDBsByBuildingAddress();
            if (fdbByBuildingAddress == null || fdbByBuildingAddress.Count == 0)
                return null;

            FDB fdb;
            if (!fdbByBuildingAddress.TryGetValue(dpBuildingAddress, out fdb))
                return null;

            return fdb;
        }

        private FDB GetFDB(long fdbId)
        {
            Dictionary<long, FDB> fdbsById = this.GetCachedFDBsById();
            return fdbsById.GetRecord(fdbId);
        }

        private Dictionary<BuildingAddress, FDB> GetCachedFDBsByBuildingAddress()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedFDBsByBuildingAddress", s_beDefinitionId, () =>
            {
                List<FDB> fdbList = this.GetCachedFDBs();
                return fdbList.ToDictionary(itm => new BuildingAddress() { Street = itm.Street, BuildingDetails = itm.BuildingDetails }, itm => itm);
            });
        }

        private Dictionary<string, FDB> GetCachedFDBsByNumber()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedFDBsByNumber", s_beDefinitionId, () =>
            {
                List<FDB> fdbList = this.GetCachedFDBs();
                return fdbList.ToDictionary(itm => itm.Number, itm => itm);
            });
        }

        private Dictionary<long, FDB> GetCachedFDBsById()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedFDBsById", s_beDefinitionId, () =>
            {
                List<FDB> fdbList = this.GetCachedFDBs();
                return fdbList.ToDictionary(itm => itm.Id, itm => itm);
            });
        }

        private List<FDB> GetCachedFDBs()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedFDBs", s_beDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(s_beDefinitionId);

                List<FDB> results = new List<FDB>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        FDB fdb = new FDB()
                        {
                            Id = (long)genericBusinessEntity.FieldValues.GetRecord("Id"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Number = (string)genericBusinessEntity.FieldValues.GetRecord("Number"),
                            Site = (long)genericBusinessEntity.FieldValues.GetRecord("Site"),
                            Region = (int)genericBusinessEntity.FieldValues.GetRecord("Region"),
                            City = (int)genericBusinessEntity.FieldValues.GetRecord("City"),
                            Town = (int)genericBusinessEntity.FieldValues.GetRecord("Town"),
                            Street = (long)genericBusinessEntity.FieldValues.GetRecord("Street"),
                            BuildingDetails = (string)genericBusinessEntity.FieldValues.GetRecord("BuildingDetails"),
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

    public class GetFDBReservationInfoInput
    {
        public NumberType NumberType { get; set; }

        public string Number { get; set; }
    }
    public class GetFDBReservationInfoOutput
    {
        FDBPortManager fdbPortManager = new FDBPortManager();

        public GetFDBReservationInfoOutput(FDB fdb)
        {
            Id = fdb.Id;
            Name = fdb.Name;
            Number = fdb.Number;
            Site = fdb.Site;
            Region = fdb.Region;
            City = fdb.City;
            Town = fdb.Town;
            Street = fdb.Street;
            BuildingDetails = fdb.BuildingDetails;
            FreePortsNb = fdbPortManager.GetFreePortsNb(fdb.Id);
            UsedPortsNb = fdbPortManager.GetUsedPortsNb(fdb.Id);
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }

        public long Site { get; set; }

        public int Region { get; set; }

        public int City { get; set; }

        public int Town { get; set; }

        public long Street { get; set; }

        public string BuildingDetails { get; set; }

        public int FreePortsNb { get; set; }

        public int UsedPortsNb { get; set; }
    }

    public class ReserveFTTHPathInput
    {
        public string FDBNumber { get; set; }
    }
    public class ReserveFTTHPathOutput
    {
        public ReserveFTTHPathOutput(FreeFTTHPath freeFTTHPath)
        {
            FDBNumber = freeFTTHPath.FDBNumber;
            FDB = freeFTTHPath.FDB;
            FDBPort = freeFTTHPath.FDBPort;
            Splitter = freeFTTHPath.Splitter;
            SplitterOutPort = freeFTTHPath.SplitterOutPort;
            SplitterInPort = freeFTTHPath.SplitterInPort;
            OLT = freeFTTHPath.OLT;
            OLTVerticalPort = freeFTTHPath.OLTVerticalPort;
            OLTHorizontalPort = freeFTTHPath.OLTHorizontalPort;
            IMS = freeFTTHPath.IMS;
            TID = freeFTTHPath.TID;
            CreatedTime = freeFTTHPath.CreatedTime;
        }

        public string FDBNumber { get; set; }

        public long FDB { get; set; }

        public long FDBPort { get; set; }

        public long Splitter { get; set; }

        public long SplitterOutPort { get; set; }

        public long SplitterInPort { get; set; }

        public long OLT { get; set; }

        public long OLTVerticalPort { get; set; }

        public long OLTHorizontalPort { get; set; }

        public long IMS { get; set; }

        public long TID { get; set; }

        public DateTime CreatedTime { get; set; }
    }

    public class GenericBEIdentifier
    {
        public GenericBEIdentifier(Guid genericBEDefinitionId, object genericBEId)
        {
            GenericBEDefinitionId = genericBEDefinitionId;
            GenericBEId = genericBEId;
        }

        public Guid GenericBEDefinitionId { get; set; }

        public object GenericBEId { get; set; }
    }
}