using Retail.NIM.Entities;
using Vanrise.Data.RDB;

namespace Retail.NIM.Data.RDB
{
    public class FreeFTTHPathDataManager : IFreeFTTHPathDataManager
    {
        #region Constants

        #region Table Aliases

        public const string AreaTableAlias = "area";
        public const string SiteTableAlias = "site";
        public const string FDBTableAlias = "fdb";
        public const string FDBPortTableAlias = "fdbPort";
        public const string SplitterTableAlias = "splitter";
        public const string SplitterOutPortTableAlias = "splitterOutPort";
        //public const string SplitterInPortTableAlias = "splitterInPort";
        public const string OLTTableAlias = "olt";
        //public const string OLTVerticalTableAlias = "oltVertical";
        //public const string OLTVerticalPortTableAlias = "oltVerticalPort";
        public const string OLTHorizontalTableAlias = "oltHorizontal";
        public const string OLTHorizontalPortTableAlias = "oltHorizontalPort";
        public const string IMSTableAlias = "ims";
        public const string IMSCardTableAlias = "imsCard";
        public const string IMSSlotTableAlias = "imsSlot";
        public const string IMSTIDTableAlias = "imsTID";

        #endregion

        #region Column Aliases

        public const string AreaIdColAlias = "AreaId";
        public const string AreaNameColAlias = "AreaName";
        public const string SiteIdColAlias = "SiteId";
        public const string SiteNameColAlias = "SiteName";

        public const string IMSIdColAlias = "IMSId";
        public const string IMSNameColAlias = "IMSName";
        public const string IMSNumberColAlias = "IMSNumber"; 
        public const string IMSCardIdColAlias = "IMSCardId";
        public const string IMSCardNameColAlias = "IMSCardName";
        public const string IMSSlotIdColAlias = "IMSSlotId";
        public const string IMSSlotNameColAlias = "IMSSlotName";
        public const string IMSTIDIdColAlias = "IMSTIDId";
        public const string IMSTIDNameColAlias = "IMSTIdName";

        public const string OLTIdColAlias = "OLTId";
        public const string OLTNameColAlias = "OLTName";
        public const string OLTHorizontalIdColAlias = "OLTHorizontalId";
        public const string OLTHorizontalNameColAlias = "OLTHorizontalName";
        public const string OLTHorizontalPortIdColAlias = "OLTHorizontalPortId";
        public const string OLTHorizontalPortNameColAlias = "OLTHorizontalPortName";
        //public const string OLTVerticalIdColAlias = "OLTVerticalId";
        //public const string OLTVerticalNameColAlias = "OLTVerticalName";
        //public const string OLTVerticalPortIdColAlias = "OLTVerticalPortId";
        //public const string OLTVerticalPortNameColAlias = "OLTVerticalPortName";

        public const string SplitterIdColAlias = "SplitterId";
        public const string SplitterNameColAlias = "SplitterName";
        //public const string SplitterInPortIdColAlias = "SplitterInPortId";
        //public const string SplitterInPortNameColAlias = "SplitterInPortName";
        public const string SplitterOutPortIdColAlias = "SplitterOutPortId";
        public const string SplitterOutPortNameColAlias = "SplitterOutPortName";

        public const string FDBIdColAlias = "FDBId";
        public const string FDBNameColAlias = "FDBName";
        public const string FDBNumberColAlias = "FDBNumber";
        public const string FDBPortIdColAlias = "FDBPortId";
        public const string FDBPortNameColAlias = "FDBPortName";

        #endregion

        #endregion

        #region Public Methods

        public FreeFTTHPath GetFreeFTTHPath(string fdbNumber)
        {
            RDBQueryContext queryContext = new RDBQueryContext(GetDataProvider());

            RDBSelectQuery selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(FDBDataManager.TABLE_NAME, FDBTableAlias, 1, true);

            var joinContext = selectQuery.Join();
            new SiteDataManager().AddJoinSiteById(joinContext, RDBJoinType.Inner, SiteTableAlias, FDBTableAlias, FDBDataManager.COL_Site, false);
            new AreaDataManager().AddJoinAreaById(joinContext, RDBJoinType.Inner, AreaTableAlias, SiteTableAlias, SiteDataManager.COL_Area, false);
            new FDBPortDataManager().AddJoinFDBPortByFDB(joinContext, RDBJoinType.Inner, FDBPortTableAlias, FDBTableAlias, FDBDataManager.COL_Id, false);

            new SplitterDataManager().AddJoinSplitterById(joinContext, RDBJoinType.Inner, SplitterTableAlias, FDBTableAlias, FDBDataManager.COL_Splitter, false);
            new SplitterOutPortDataManager().AddJoinSplitterOutPortBySplitter(joinContext, RDBJoinType.Inner, SplitterOutPortTableAlias, SplitterTableAlias, SplitterDataManager.COL_Id, false);
            //new SplitterInPortDataManager().AddJoinSplitterInPortBySplitter(joinContext, RDBJoinType.Inner, SplitterInPortTableAlias, SplitterTableAlias, SplitterDataManager.COL_Id, false);

            new OLTDataManager().AddJoinOLTById(joinContext, RDBJoinType.Inner, OLTTableAlias, SplitterTableAlias, SplitterDataManager.COL_OLT, false);
            //new OLTVerticalDataManager().AddJoinOLTVerticalByOLT(joinContext, RDBJoinType.Inner, OLTVerticalTableAlias, OLTTableAlias, OLTDataManager.COL_Id, false);
            //new OLTVerticalPortDataManager().AddJoinOLTVerticalPortByOLTVertical(joinContext, RDBJoinType.Inner, OLTVerticalPortTableAlias, OLTVerticalTableAlias, OLTVerticalDataManager.COL_Id, false);
            new OLTHorizontalDataManager().AddJoinOLTHorizontalByOLT(joinContext, RDBJoinType.Inner, OLTHorizontalTableAlias, OLTTableAlias, OLTDataManager.COL_Id, false);
            new OLTHorizontalPortDataManager().AddJoinOLTHorizontalPortByOLTHorizontal(joinContext, RDBJoinType.Inner, OLTHorizontalPortTableAlias, OLTHorizontalTableAlias, OLTHorizontalDataManager.COL_Id, false);

            new IMSDataManager().AddJoinIMSById(joinContext, RDBJoinType.Inner, IMSTableAlias, OLTTableAlias, OLTDataManager.COL_IMS, false);
            new IMSCardDataManager().AddJoinIMSCardByIMS(joinContext, RDBJoinType.Inner, IMSCardTableAlias, IMSTableAlias, IMSDataManager.COL_Id, false);
            new IMSSlotDataManager().AddJoinIMSSlotByIMSCard(joinContext, RDBJoinType.Inner, IMSSlotTableAlias, IMSCardTableAlias, IMSCardDataManager.COL_Id, false);
            new IMSTIDDataManager().AddJoinIMSTIDByIMSSlot(joinContext, RDBJoinType.Inner, IMSTIDTableAlias, IMSSlotTableAlias, IMSSlotDataManager.COL_Id, false);

            selectQuery.SelectColumns().Column(AreaTableAlias, AreaDataManager.COL_Id, AreaIdColAlias);
            selectQuery.SelectColumns().Column(AreaTableAlias, AreaDataManager.COL_Name, AreaNameColAlias);
            selectQuery.SelectColumns().Column(SiteTableAlias, SiteDataManager.COL_Id, SiteIdColAlias);
            selectQuery.SelectColumns().Column(SiteTableAlias, SiteDataManager.COL_Name, SiteNameColAlias);

            selectQuery.SelectColumns().Column(IMSTableAlias, IMSDataManager.COL_Id, IMSIdColAlias);
            selectQuery.SelectColumns().Column(IMSTableAlias, IMSDataManager.COL_Name, IMSNameColAlias);
            selectQuery.SelectColumns().Column(IMSTableAlias, IMSDataManager.COL_Number, IMSNumberColAlias);
            selectQuery.SelectColumns().Column(IMSCardTableAlias, IMSCardDataManager.COL_Id, IMSCardIdColAlias);
            selectQuery.SelectColumns().Column(IMSCardTableAlias, IMSCardDataManager.COL_Name, IMSCardNameColAlias);
            selectQuery.SelectColumns().Column(IMSSlotTableAlias, IMSSlotDataManager.COL_Id, IMSSlotIdColAlias);
            selectQuery.SelectColumns().Column(IMSSlotTableAlias, IMSSlotDataManager.COL_Name, IMSSlotNameColAlias);
            selectQuery.SelectColumns().Column(IMSTIDTableAlias, IMSTIDDataManager.COL_Id, IMSTIDIdColAlias);
            selectQuery.SelectColumns().Column(IMSTIDTableAlias, IMSTIDDataManager.COL_Name, IMSTIDNameColAlias);

            selectQuery.SelectColumns().Column(OLTTableAlias, OLTDataManager.COL_Id, OLTIdColAlias);
            selectQuery.SelectColumns().Column(OLTTableAlias, OLTDataManager.COL_Name, OLTNameColAlias);
            selectQuery.SelectColumns().Column(OLTHorizontalTableAlias, OLTHorizontalDataManager.COL_Id, OLTHorizontalIdColAlias);
            selectQuery.SelectColumns().Column(OLTHorizontalTableAlias, OLTHorizontalDataManager.COL_Name, OLTHorizontalNameColAlias);
            selectQuery.SelectColumns().Column(OLTHorizontalPortTableAlias, OLTHorizontalPortDataManager.COL_Id, OLTHorizontalPortIdColAlias);
            selectQuery.SelectColumns().Column(OLTHorizontalPortTableAlias, OLTHorizontalPortDataManager.COL_Name, OLTHorizontalPortNameColAlias);
            //selectQuery.SelectColumns().Column(OLTVerticalTableAlias, OLTVerticalDataManager.COL_Id, OLTVerticalIdColAlias);
            //selectQuery.SelectColumns().Column(OLTVerticalTableAlias, OLTVerticalDataManager.COL_Name, OLTVerticalNameColAlias);
            //selectQuery.SelectColumns().Column(OLTVerticalPortTableAlias, OLTVerticalPortDataManager.COL_Id, OLTVerticalPortIdColAlias);
            //selectQuery.SelectColumns().Column(OLTVerticalPortTableAlias, OLTVerticalPortDataManager.COL_Name, OLTVerticalPortNameColAlias);

            selectQuery.SelectColumns().Column(SplitterTableAlias, SplitterDataManager.COL_Id, SplitterIdColAlias);
            selectQuery.SelectColumns().Column(SplitterTableAlias, SplitterDataManager.COL_Name, SplitterNameColAlias);
            //selectQuery.SelectColumns().Column(SplitterInPortTableAlias, SplitterInPortDataManager.COL_Id, SplitterInPortIdColAlias);
            //selectQuery.SelectColumns().Column(SplitterInPortTableAlias, SplitterInPortDataManager.COL_Name, SplitterInPortNameColAlias);
            selectQuery.SelectColumns().Column(SplitterOutPortTableAlias, SplitterOutPortDataManager.COL_Id, SplitterOutPortIdColAlias);
            selectQuery.SelectColumns().Column(SplitterOutPortTableAlias, SplitterOutPortDataManager.COL_Name, SplitterOutPortNameColAlias);

            selectQuery.SelectColumns().Column(FDBTableAlias, FDBDataManager.COL_Id, FDBIdColAlias);
            selectQuery.SelectColumns().Column(FDBTableAlias, FDBDataManager.COL_Name, FDBNameColAlias);
            selectQuery.SelectColumns().Column(FDBTableAlias, FDBDataManager.COL_Number, FDBNumberColAlias);
            selectQuery.SelectColumns().Column(FDBPortTableAlias, FDBPortDataManager.COL_Id, FDBPortIdColAlias);
            selectQuery.SelectColumns().Column(FDBPortTableAlias, FDBPortDataManager.COL_Name, FDBPortNameColAlias);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(FDBTableAlias, FDBDataManager.COL_Number).Value(fdbNumber);
            whereContext.EqualsCondition(FDBPortTableAlias, FDBPortDataManager.COL_Status).Value(PortStatus.Free);
            whereContext.EqualsCondition(SplitterOutPortTableAlias, SplitterOutPortDataManager.COL_Status).Value(PortStatus.Free);
            //whereContext.EqualsCondition(SplitterInPortTableAlias, SplitterInPortDataManager.COL_Status).Value(PortStatus.Free);
            //whereContext.EqualsCondition(OLTVerticalPortTableAlias, OLTVerticalPortDataManager.COL_Status).Value(PortStatus.Free);
            whereContext.EqualsCondition(OLTHorizontalPortTableAlias, OLTHorizontalPortDataManager.COL_Status).Value(PortStatus.Free);
            whereContext.EqualsCondition(IMSTIDTableAlias, IMSTIDDataManager.COL_Status).Value(PortStatus.Free);

            return queryContext.GetItem<FreeFTTHPath>(FreeFTTHPathMapper);
        }

        #endregion

        #region Private Methods

        private BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Retail_NIM", "NIMDBConnStringKey", "NIMDBConnString");
        }

        #endregion

        #region Mappers

        private FreeFTTHPath FreeFTTHPathMapper(IRDBDataReader reader)
        {
            FreeFTTHPath freeFTTHPath = new FreeFTTHPath
            {
                AreaId = reader.GetInt(AreaIdColAlias),
                AreaName = reader.GetString(AreaNameColAlias),
                SiteId = reader.GetInt(SiteIdColAlias),
                SiteName = reader.GetString(SiteNameColAlias),

                IMSId = reader.GetInt(IMSIdColAlias),
                IMSName = reader.GetString(IMSNameColAlias),
                IMSNumber = reader.GetString(IMSNumberColAlias),
                IMSCardId = reader.GetInt(IMSCardIdColAlias),
                IMSCardName = reader.GetString(IMSCardNameColAlias),
                IMSSlotId = reader.GetInt(IMSSlotIdColAlias),
                IMSSlotName = reader.GetString(IMSSlotNameColAlias),
                IMSTIDId = reader.GetInt(IMSTIDIdColAlias),
                IMSTIDName = reader.GetString(IMSTIDNameColAlias),

                OLTId = reader.GetInt(OLTIdColAlias),
                OLTName = reader.GetString(OLTNameColAlias),
                OLTHorizontalId = reader.GetInt(OLTHorizontalIdColAlias),
                OLTHorizontalName = reader.GetString(OLTHorizontalNameColAlias),
                OLTHorizontalPortId = reader.GetInt(OLTHorizontalPortIdColAlias),
                OLTHorizontalPortName = reader.GetString(OLTHorizontalPortNameColAlias),
                //OLTVerticalId = reader.GetInt(OLTVerticalIdColAlias),
                //OLTVerticalName = reader.GetString(OLTVerticalNameColAlias),
                //OLTVerticalPortId = reader.GetInt(OLTVerticalPortIdColAlias),
                //OLTVerticalPortName = reader.GetString(OLTVerticalPortNameColAlias),

                SplitterId = reader.GetInt(SplitterIdColAlias),
                SplitterName = reader.GetString(SplitterNameColAlias),
                //SplitterInPortId = reader.GetInt(SplitterInPortIdColAlias),
                //SplitterInPortName = reader.GetString(SplitterInPortNameColAlias),
                SplitterOutPortId = reader.GetInt(SplitterOutPortIdColAlias),
                SplitterOutPortName = reader.GetString(SplitterOutPortNameColAlias),

                FDBId = reader.GetInt(FDBIdColAlias),
                FDBName = reader.GetString(FDBNameColAlias),
                FDBNumber = reader.GetString(FDBNumberColAlias),
                FDBPortId = reader.GetInt(FDBPortIdColAlias),
                FDBPortName = reader.GetString(FDBPortNameColAlias)
            };

            return freeFTTHPath;
        }

        #endregion
    }
}