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

            List<GenericBEIdentifier> genericBEPortIdentifierList = BuildGenericBEPortIdentifierList(freeFTTHPath);

            foreach (var genericBEPortIdentifier in genericBEPortIdentifierList)
                UpdateGenericBEPortStatus(genericBEPortIdentifier, PortStatus.TemporaryReserved);

            return new ReserveFTTHPathOutput(freeFTTHPath);
        }

        public AddConnectionOutput AddConnection(AddConnectionInput input)
        {
            input.ThrowIfNull("AddConnectionInput");

            Dictionary<ConnectionType, ConnectionBEDefinitionData> connectionBEDefinitionDataDict = GetConnectionBEDefinitionDataDict();
            ConnectionBEDefinitionData connectionBEDefinitionData = connectionBEDefinitionDataDict.GetRecord(input.ConnectionType);

            string port1Name, port2Name;

            switch (input.ConnectionType)
            {
                case ConnectionType.IMSPhoneNumberTID: port1Name = "PhoneNumber"; port2Name = "TID"; break;
                case ConnectionType.IMSOLT: port1Name = "TID"; port2Name = "OLTHorizontalPort"; break;
                case ConnectionType.OLTSplitterLogical: port1Name = "OLTHorizontalPort"; port2Name = "SplitterOutPort"; break;
                case ConnectionType.SplitterFDB: port1Name = "SplitterOutPort"; port2Name = "FDBPort"; break;
                default: throw new NotSupportedException($"ConnectionType {input.ConnectionType} not supported.");
            }

            InsertGenericBEConnector(connectionBEDefinitionData.ConnectionDefinitionId, port1Name, input.Port1Id, port2Name, input.Port2Id);

            if (connectionBEDefinitionData.Port1DefinitionId.HasValue)
                UpdateGenericBEPortStatus(new GenericBEIdentifier(connectionBEDefinitionData.Port1DefinitionId.Value, input.Port1Id), PortStatus.Used);

            if (connectionBEDefinitionData.Port2DefinitionId.HasValue)
                UpdateGenericBEPortStatus(new GenericBEIdentifier(connectionBEDefinitionData.Port2DefinitionId.Value, input.Port2Id), PortStatus.Used);

            return new AddConnectionOutput() { OperationSucceeded = true };
        }

        #endregion

        #region Private Methods

        private void InsertGenericBEConnector(Guid connectionDefinitionId, string port1Name, long port1Id, string port2Name, long port2Id)
        {
            GenericBusinessEntityToAdd genericBusinessEntityToAdd = new GenericBusinessEntityToAdd()
            {
                BusinessEntityDefinitionId = connectionDefinitionId,
                FieldValues = new Dictionary<string, object>()
            };
            genericBusinessEntityToAdd.FieldValues.Add(port1Name, port1Id);
            genericBusinessEntityToAdd.FieldValues.Add(port2Name, port2Id);
            new GenericBusinessEntityManager().AddGenericBusinessEntity(genericBusinessEntityToAdd, null);
        }

        private void UpdateGenericBEPortStatus(GenericBEIdentifier genericBEPortIdentifier, Guid statusId)
        {
            GenericBusinessEntityToUpdate genericBusinessEntityToUpdate = new GenericBusinessEntityToUpdate()
            {
                BusinessEntityDefinitionId = genericBEPortIdentifier.GenericBEDefinitionId,
                GenericBusinessEntityId = genericBEPortIdentifier.GenericBEId,
                FieldValues = new Dictionary<string, object>()
            };
            genericBusinessEntityToUpdate.FieldValues.Add("Status", statusId);
            new GenericBusinessEntityManager().UpdateGenericBusinessEntity(genericBusinessEntityToUpdate, null);
        }

        private Dictionary<ConnectionType, ConnectionBEDefinitionData> GetConnectionBEDefinitionDataDict()
        {
            var connectionBEDefinitionData = new Dictionary<ConnectionType, ConnectionBEDefinitionData>();

            var imsPhoneNumberTIDConnectorData = new ConnectionBEDefinitionData(IMSPhoneNumberTIDConnectorManager.s_beDefinitionId, IMSPhoneNumberManager.s_beDefinitionId);
            connectionBEDefinitionData.Add(ConnectionType.IMSPhoneNumberTID, imsPhoneNumberTIDConnectorData);

            var imsOLTConnectorData = new ConnectionBEDefinitionData(IMSOLTConnectorManager.s_beDefinitionId, IMSTIDManager.s_beDefinitionId, OLTHorizontalPortManager.s_beDefinitionId);
            connectionBEDefinitionData.Add(ConnectionType.IMSOLT, imsOLTConnectorData);

            var oltSplitterLogicalConnectorData = new ConnectionBEDefinitionData(OLTSplitterLogicalConnectorManager.s_beDefinitionId);
            connectionBEDefinitionData.Add(ConnectionType.OLTSplitterLogical, oltSplitterLogicalConnectorData);

            var splitterFDBConnectorData = new ConnectionBEDefinitionData(SplitterFDBConnectorManager.s_beDefinitionId, SplitterOutPortManager.s_beDefinitionId, FDBPortManager.s_beDefinitionId);
            connectionBEDefinitionData.Add(ConnectionType.SplitterFDB, splitterFDBConnectorData);

            return connectionBEDefinitionData;
        }

        private List<GenericBEIdentifier> BuildGenericBEPortIdentifierList(FreeFTTHPath freeFTTHPath)
        {
            List<GenericBEIdentifier> genericBEIdentifierList = new List<GenericBEIdentifier>();
            genericBEIdentifierList.Add(new GenericBEIdentifier(FDBPortManager.s_beDefinitionId, freeFTTHPath.FDBPortId));
            genericBEIdentifierList.Add(new GenericBEIdentifier(SplitterOutPortManager.s_beDefinitionId, freeFTTHPath.SplitterOutPortId));
            //genericBEIdentifierList.Add(new GenericBEIdentifier(SplitterInPortManager.s_beDefinitionId, freeFTTHPath.SplitterInPortId));
            //genericBEIdentifierList.Add(new GenericBEIdentifier(OLTVerticalPortManager.s_beDefinitionId, freeFTTHPath.OLTVerticalPortId));
            genericBEIdentifierList.Add(new GenericBEIdentifier(OLTHorizontalPortManager.s_beDefinitionId, freeFTTHPath.OLTHorizontalPortId));
            genericBEIdentifierList.Add(new GenericBEIdentifier(IMSTIDManager.s_beDefinitionId, freeFTTHPath.IMSTIDId));
            return genericBEIdentifierList;
        }

        private FDB GetFDBByNearestPhoneNumber(string nearestPhoneNumber)
        {
            if (string.IsNullOrEmpty(nearestPhoneNumber))
                return null;

            FTTHPath ftthPath = new FTTHPathManager().GetFTTHPathByFullPhoneNumber(nearestPhoneNumber);
            if(ftthPath != null)
            {
                FDB fdb = GetFDB(ftthPath.FDB);
                if (fdb != null)
                    return fdb;
            }

            CopperPath copperPath = new CopperPathManager().GetCopperPathByFullPhoneNumber(nearestPhoneNumber);
            if (copperPath != null)
            {
                DP dp = new DPManager().GetDPById(copperPath.DP);
                dp.ThrowIfNull("dp", copperPath.DP);

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
            AreaId = freeFTTHPath.AreaId;
            AreaName = freeFTTHPath.AreaName;
            SiteId = freeFTTHPath.SiteId;
            SiteName = freeFTTHPath.SiteName;
            IMSId = freeFTTHPath.IMSId;
            IMSName = freeFTTHPath.IMSName;
            IMSNumber = freeFTTHPath.IMSNumber;
            IMSCardId = freeFTTHPath.IMSCardId;
            IMSCardName = freeFTTHPath.IMSCardName;
            IMSSlotId = freeFTTHPath.IMSSlotId;
            IMSSlotName = freeFTTHPath.IMSSlotName;
            IMSTIDId = freeFTTHPath.IMSTIDId;
            IMSTIDName = freeFTTHPath.IMSTIDName;
            OLTId = freeFTTHPath.OLTId;
            OLTName = freeFTTHPath.OLTName;
            OLTHorizontalId = freeFTTHPath.OLTHorizontalId;
            OLTHorizontalName = freeFTTHPath.OLTHorizontalName;
            OLTHorizontalPortId = freeFTTHPath.OLTHorizontalPortId;
            OLTHorizontalPortName = freeFTTHPath.OLTHorizontalPortName;
            SplitterId = freeFTTHPath.SplitterId;
            SplitterName = freeFTTHPath.SplitterName;
            SplitterOutPortId = freeFTTHPath.SplitterOutPortId;
            SplitterOutPortName = freeFTTHPath.SplitterOutPortName;
            FDBId = freeFTTHPath.FDBId;
            FDBName = freeFTTHPath.FDBName;
            FDBNumber = freeFTTHPath.FDBNumber;
            FDBPortId = freeFTTHPath.FDBPortId;
            FDBPortName = freeFTTHPath.FDBPortName;
        }

        public long AreaId { get; set; }
        public string AreaName { get; set; }
        public long SiteId { get; set; }
        public string SiteName { get; set; }
        public long IMSId { get; set; }
        public string IMSName { get; set; }
        public string IMSNumber { get; set; }
        public long IMSCardId { get; set; }
        public string IMSCardName { get; set; }
        public long IMSSlotId { get; set; }
        public string IMSSlotName { get; set; }
        public long IMSTIDId { get; set; }
        public string IMSTIDName { get; set; }
        public long OLTId { get; set; }
        public string OLTName { get; set; }
        public long OLTHorizontalId { get; set; }
        public string OLTHorizontalName { get; set; }
        public long OLTHorizontalPortId { get; set; }
        public string OLTHorizontalPortName { get; set; }
        public long SplitterId { get; set; }
        public string SplitterName { get; set; }
        public long SplitterOutPortId { get; set; }
        public string SplitterOutPortName { get; set; }
        public long FDBId { get; set; }
        public string FDBName { get; set; }
        public string FDBNumber { get; set; }
        public long FDBPortId { get; set; }
        public string FDBPortName { get; set; }
    }

    public class AddConnectionInput
    {
        public ConnectionType ConnectionType { get; set; }

        public long Port1Id { get; set; }

        public long Port2Id { get; set; }
    }
    public class AddConnectionOutput
    {
        public bool OperationSucceeded { get; set; }

        public string ErrorMessage { get; set; }
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

    public class ConnectionBEDefinitionData
    {
        public ConnectionBEDefinitionData(Guid connectionDefinitionId, Guid? port1DefinitionId = null, Guid? port2DefinitionId = null)
        {
            ConnectionDefinitionId = connectionDefinitionId;
            Port1DefinitionId = port1DefinitionId;
            Port2DefinitionId = port2DefinitionId;
        }

        public Guid ConnectionDefinitionId { get; set; }

        public Guid? Port1DefinitionId { get; set; }

        public Guid? Port2DefinitionId { get; set; }
    }
}