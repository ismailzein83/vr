using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABS.Components.CodeComparison
{
    public class ComparisonOptions
    {
        public string DefaultSuppliers { get; set; }
        public string MailRecepients { get; set; }
        public short? Threshold { get; set; }
        public string NameSupplierId { get; set; }
        public string MobileKeywords { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0}|{1}|{2}|{3}|{4}",
                DefaultSuppliers, MailRecepients, Threshold, NameSupplierId, MobileKeywords);
        }
        public ComparisonOptions()
        {

        }
        public ComparisonOptions(string toParse)
        {
            string[] props = toParse.Split('|');
            if (props.Length != 5)
            {
                DefaultSuppliers = null;
                MailRecepients = null;
                Threshold = null;
                NameSupplierId = null;
                MobileKeywords = null;
            }
            else
            {
                this.DefaultSuppliers = props[0];
                this.MailRecepients = props[1];
                this.Threshold = short.Parse(props[2]);
                this.NameSupplierId = props[3];
                this.MobileKeywords = props[4];
            }
        }
    }


    public class CodeMatching
    {
        public string Code { get; set; }
        public string MatchingCode { get; set; }
        public string MatchingCodeZoneName { get; set; }
        public CarrierAccount MatchingCodeSupplier { get; set; }
        //public Comparison Comparison { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var MatchObject = ((CodeMatching)obj);

            if (MatchObject == null)
                return false;

            if (!(
                MatchObject.Code == this.Code && MatchObject.MatchingCode == this.MatchingCode
                && MatchObject.MatchingCodeZoneName == this.MatchingCodeZoneName
                && MatchObject.MatchingCodeSupplier == this.MatchingCodeSupplier))
                return false;
            else
                return true;
        }

        public override int GetHashCode()
        {
            if (MatchingCodeSupplier != null)
                return (Code + MatchingCodeZoneName + MatchingCode + MatchingCodeSupplier.CarrierAccountID).GetHashCode();
            else
                return base.GetHashCode();
        }
    }

    /// <summary>
    /// A class representing the comparison between a specific Code and the codes of
    /// certain suppliers
    /// </summary>
    public class ComparedCode : Dictionary<TABS.CarrierAccount, TABS.Code>
    {
        public string Code { get; protected set; }
        /// <summary>
        /// The whole comparison that this code belongs to
        /// </summary>
        public Comparison Comparison { get; protected set; }
        public TABS.Code SaleCode { get; protected set; }
        public TABS.Zone SaleZone { get; protected set; }

        public ComparedCode(Comparison comparison, string code)
        {
            this.Comparison = comparison;
            this.Code = code;
        }

        public void AddAndCheck(TABS.CarrierAccount supplier, TABS.Code code)
        {
            if (supplier.Equals(TABS.CarrierAccount.SYSTEM))
            {
                SaleCode = code;
                SaleZone = code.Zone;
            }
            else
                this[supplier] = code;
        }

        // Computed Properties
        public int SupplyCount { get { return this.Values.Where(c => c.Value.Equals(Code)).Count(); } }
        public int MissingSupplyCount { get { return Comparison.Suppliers.Count - this.SupplyCount; } }
        public bool IsSaleMissing { get { return this.SaleCode == null || this.SaleCode.Value != this.Code; } }
        public bool IsSold { get { return this.SaleCode != null && this.SaleCode.Value == this.Code; } }
        public bool IsDeleteCandidate { get { if (IsSold) { return MissingSupplyCount >= Comparison.ChangeCountThreshold; } else return false; } }
        public bool IsNewCandidate { get { if (IsSaleMissing) { return SupplyCount >= Comparison.ChangeCountThreshold; } else return false; } }
        public string CodeGroupName { get { return TABS.CodeGroup.FindForCode(Code).Name; } }
        public string ActionFlag { get { return IsDeleteCandidate ? "D" : IsNewCandidate ? "N" : string.Empty; } }

        public string SaleZoneName { get { return SaleZone != null ? SaleZone.Name : null; } }
        public string SaleCodeValue { get { return SaleCode != null ? SaleCode.Value : null; } }

        /// <summary>
        /// Indicates if there is any slight difference in code between sale and purchase
        /// </summary>
        public bool HasDifference
        {
            get
            {
                if (SupplyCount < Comparison.Suppliers.Count || IsSaleMissing) return true;
                foreach (var supplyCode in this.Values)
                    if (supplyCode.Value != Code)
                        return true;
                return false;

            }
        }

        public string GetCodeValue(TABS.CarrierAccount supplier)
        {
            TABS.Code code;
            if (this.TryGetValue(supplier, out code))
                return code.Value;
            else
                return null;
        }

        public string GetZoneName(TABS.CarrierAccount supplier)
        {
            TABS.Code code;
            if (this.TryGetValue(supplier, out code))
                return code.Zone.Name;
            else
                return null;
        }
    }
    /// <summary>
    /// A class representing a comparison between a set of codes against codes of 
    /// specified suppliers. The comparison is defined by a CodeFilter that limits
    /// the subset of codes to consider and a list of suppliers to compare against.
    /// </summary>
    public class Comparison : Dictionary<string, ComparedCode>
    {

        static log4net.ILog log = log4net.LogManager.GetLogger("Code Comparison");

        List<CodeMatching> Matches = new List<CodeMatching>();
        public List<TABS.CarrierAccount> Suppliers { get; protected set; }
        public string CodeFilter { get; protected set; }
        /// <summary>
        /// This defines the count threshold at which the comparison is flagged for processing
        /// </summary>
        public int ChangeCountThreshold { get; protected set; }
        protected void Add(RoutePool.CodeRoute codeRoute)
        {
            ComparedCode comparedCode = null;
            if (!this.TryGetValue(codeRoute.Code, out comparedCode))
            {
                comparedCode = new ComparedCode(this, codeRoute.Code);
                this.Add(codeRoute.Code, comparedCode);
            }
            if (codeRoute.SaleCode != null)
                comparedCode.AddAndCheck(codeRoute.SaleCode.Supplier, codeRoute.SaleCode);
            foreach (Code c in codeRoute.SupplyRates.Keys)
                if (Suppliers.Contains(c.Zone.Supplier))
                    comparedCode.AddAndCheck(c.Zone.Supplier, c);
            //foreach (Rate c in codeRoute.SupplierRates)
            //    if (Suppliers.Contains(c.Zone.Supplier))
            //        comparedCode.AddAndCheck(c.Zone.Supplier, codeRoute.);
        }

        public Comparison(string codeFilter, List<TABS.CarrierAccount> suppliers, int changeThresholdCount)
        {
            this.CodeFilter = codeFilter;
            this.Suppliers = suppliers;
            this.ChangeCountThreshold = changeThresholdCount > suppliers.Count ? suppliers.Count : changeThresholdCount;
            StringBuilder sb = new StringBuilder();
            sb.Append("'SYS',");
            foreach (var supp in suppliers)
            {
                sb.AppendFormat(@"'{0}',", supp.CarrierAccountID);
            }
            string value = sb.ToString().TrimEnd(',');


            string query = string.Format(@"WITH ZoneNames AS (SELECT [ZoneID],[Name] FROM ZONE WITH ( NOLOCK) WHERE [SupplierID] IN ({0}) AND EndEffectiveDate IS NULL)

                             ,CodeMatchCTE AS (SELECT 
                                [Code],[MatchingCode],[SupplierCodeID],[SupplierZoneID],[SupplierID] 
                             FROM
                                CodeMatchForCodeComparison WITH (NOLOCK) 
                             WHERE {1} [SupplierID] in ({0}))
                             
                             SELECT CC.[Code],CC.[MatchingCode],Z.[Name],CC.[SupplierID] 
                             FROM
                                CodeMatchCTE CC
                             JOIN
                                ZoneNames Z
                             ON CC.[SupplierZoneID] = Z.[ZoneID]", value, string.IsNullOrEmpty(codeFilter) ? string.Empty : string.Format("[Code] LIKE '{0}%' AND", codeFilter));



            try
            {
                var reader = TABS.DataHelper.ExecuteReader(query);

                if (reader != null)
                {
                    using (reader)
                    {
                        Func<IDataReader, int, string> SafeGetString = (rdr, index) => { return rdr.IsDBNull(index) ? "" : rdr.GetString(index); };

                        while (reader.Read())
                        {
                            CarrierAccount C;
                            if (TABS.CarrierAccount.All.TryGetValue(reader.GetString(3), out C))
                            {
                                CodeMatching Match = new CodeMatching();
                                Match.Code = SafeGetString(reader, 0);
                                Match.MatchingCode = SafeGetString(reader, 1);
                                Match.MatchingCodeZoneName = SafeGetString(reader, 2);
                                Match.MatchingCodeSupplier = C;
                                Matches.Add(Match);
                            }
                        }

                        SafeGetString = null;
                    }
                }


            }
            catch (Exception ex)
            {
                log.Error("Error loading matches from database :", ex);
            }

        }

        public DataTable ToFlatDataTable(TABS.CarrierAccount SupplierToUseDestinationNames, bool ShowOnlySaleAndSupplierCodes)
        {
            var Supplier = SupplierToUseDestinationNames;
            DataTable FlatDataTable = new DataTable();
            int nameSupplierIndex = -1;
            FlatDataTable.Columns.Add("Code");
            FlatDataTable.Columns.Add("Group");
            FlatDataTable.Columns.Add("Sale Zone");
            FlatDataTable.Columns.Add("Sale Code");
            if (Supplier != null)
            {
                foreach (TABS.CarrierAccount supplier in this.Suppliers)
                {
                    FlatDataTable.Columns.Add(supplier.Name);
                    if (supplier.Equals(Supplier))
                        nameSupplierIndex = FlatDataTable.Columns.Count - 1;
                    FlatDataTable.Columns.Add(supplier.Name + " Code");
                }
            }
            else
            {
                foreach (TABS.CarrierAccount supplier in this.Suppliers)
                {
                    FlatDataTable.Columns.Add(supplier.Name);
                    FlatDataTable.Columns.Add(supplier.Name + " Code");
                }
            }

            FlatDataTable.Columns.Add("SupplyCount");
            FlatDataTable.Columns.Add("MissingSupplyCount");
            FlatDataTable.Columns.Add("HasDifference");
            FlatDataTable.Columns.Add("IsNewCandidate");
            FlatDataTable.Columns.Add("IsDeleteCandidate");
            FlatDataTable.Columns.Add("Action");
            FlatDataTable.Columns.Add("Destination");


            var MatchesGroupedByCode = Matches.OrderBy(cm => cm.Code).GroupBy(cm => cm.Code);

            foreach (var Group in MatchesGroupedByCode)
            {
                DataRow Row = FlatDataTable.NewRow();
                string SaleZoneName = "";

                bool IsCodeGroupSold = false;
                bool IsSaleCodeMatchingExactly = false;
                bool HasDifference = false;

                int ExactMatchingSuppliersCount = 0;
                Row["Code"] = Group.Key;
                Row["Group"] = TABS.CodeGroup.FindForCode(Group.Key).Name;

                foreach (CodeMatching Match in Group)
                {
                    if (Match.MatchingCodeSupplier.Equals(TABS.CarrierAccount.SYSTEM))
                    {
                        IsCodeGroupSold = true;
                        if (Match.MatchingCode == Match.Code)
                            IsSaleCodeMatchingExactly = true;
                        else
                            HasDifference = true;

                        SaleZoneName = Match.MatchingCodeZoneName;
                        Row["Sale Zone"] = Match.MatchingCodeZoneName;
                        Row["Sale Code"] = Match.MatchingCode;
                    }
                    else
                    {
                        if (Match.MatchingCode == Match.Code)
                            ExactMatchingSuppliersCount++;

                        Row[Match.MatchingCodeSupplier.Name] = Match.MatchingCodeZoneName;
                        Row[Match.MatchingCodeSupplier.Name + " Code"] = Match.MatchingCode;
                    }
                }

                int MissingSupplyCount = this.Suppliers.Count - ExactMatchingSuppliersCount;
                bool IsSold = IsCodeGroupSold && IsSaleCodeMatchingExactly;
                bool IsDeleteCandidate = IsSold ? MissingSupplyCount >= this.ChangeCountThreshold : false;
                bool IsNewCandidate = !IsSold ? ExactMatchingSuppliersCount >= this.ChangeCountThreshold : false;
                HasDifference = (MissingSupplyCount > 0 || !IsSold) ? true : HasDifference;
                Row["SupplyCount"] = ExactMatchingSuppliersCount;
                Row["MissingSupplyCount"] = MissingSupplyCount;
                Row["HasDifference"] = HasDifference ? "*" : string.Empty;
                Row["IsNewCandidate"] = IsNewCandidate ? "*" : string.Empty;
                Row["IsDeleteCandidate"] = IsDeleteCandidate ? "*" : string.Empty;
                Row["Action"] = IsDeleteCandidate ? "D" : IsNewCandidate ? "N" : string.Empty;
                if (IsNewCandidate && nameSupplierIndex > -1) Row["Destination"] = Row[nameSupplierIndex];
                if (IsDeleteCandidate) Row["Destination"] = SaleZoneName;

                if (ShowOnlySaleAndSupplierCodes)
                {
                    if (IsNewCandidate || IsDeleteCandidate)
                        FlatDataTable.Rows.Add(Row);
                }
                else
                {
                    FlatDataTable.Rows.Add(Row);
                }
            }

            return FlatDataTable;
        }

        public static DataTable GetCodeComparison(string TableName,int PageSize, string codeFilter, List<TABS.CarrierAccount> suppliers, int changeThresholdCount, TABS.CarrierAccount SupplierToUseDestinationNames, bool ShowOnlySaleAndSupplierCodes, out int RecordCount)
        {
            RecordCount = 0;

            var Comparison = new TABS.Components.CodeComparison.Comparison(codeFilter, suppliers, changeThresholdCount);
            var CompDataTable = Comparison.ToFlatDataTable(SupplierToUseDestinationNames, ShowOnlySaleAndSupplierCodes);
            //var TableName = "RgCodeComparison" + System.Web.HttpContext.Current.Session.SessionID;

            StringBuilder sb = new StringBuilder();
            
            sb.AppendFormat(@"Declare @exists bit                   
                      SET @exists=dbo.CheckGlobalTableExists('{0}')
                      
                      IF(@Exists = 1)
		                   DROP TABLE TempDB.dbo.[{0}]",TableName);

            sb.AppendFormat("{0}", Environment.NewLine);
            sb.AppendFormat("{0}", Environment.NewLine);

            sb.AppendFormat(@"CREATE TABLE TempDB.dbo.[{0}]", TableName);
            sb.AppendFormat("{0}", Environment.NewLine);
            sb.Append("     (");
            sb.AppendFormat("{0}",Environment.NewLine);

            var lastcolumn = CompDataTable.Columns[CompDataTable.Columns.Count - 1];

            foreach (DataColumn column in CompDataTable.Columns)
            {
                sb.AppendFormat("           [{0}]",column.ColumnName);
                sb.Append(" ");
                sb.Append("nvarchar(100)");

                if (column != lastcolumn)
                    sb.Append(",");

                sb.AppendFormat("{0}", Environment.NewLine);
            }
             //To remove the last comma and breakline
            sb.Append("     )");

            using (TABS.Components.BulkManager bm = new BulkManager(log))
            {
                using (var transaction = bm.Connection.BeginTransaction())
                {
                    try
                    {
                        var CreateTableCommand = bm.Connection.CreateCommand();
                        CreateTableCommand.CommandText = sb.ToString();
                        CreateTableCommand.Transaction = transaction;
                        CreateTableCommand.ExecuteNonQuery();

                        bm.Write
                            (
                                CompDataTable,
                                transaction,
                                System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls,
                                string.Format("TempDB.dbo.[{0}]", TableName)
                            );

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        transaction.Rollback();
                    }
                }
            }

            sb.Length = 0;

            sb.AppendFormat(@"SELECT COUNT(1) FROM TempDB.dbo.[{0}];
                          WITH FINAL AS 
                            (
                               select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) ) AS rowNumber
                               from TempDB.dbo.[{0}]
                             )
                          SELECT * FROM FINAL WHERE RowNumber BETWEEN 1 AND {1}", TableName, PageSize);

            var resultDataset = TABS.DataHelper.GetDataSet(sb.ToString());

            RecordCount = int.Parse(resultDataset.Tables[0].Rows[0][0].ToString());

            return resultDataset.Tables[1];
        }


        //public DataTable ToFlatDataTable(TABS.CarrierAccount nameSupplier)
        //{
        //    DataTable data = new DataTable();
        //    int nameSupplierIndex = -1;
        //    data.Columns.Add("Code");
        //    data.Columns.Add("Group");
        //    data.Columns.Add("Sale Zone");
        //    data.Columns.Add("Sale Code");
        //    if (nameSupplier != null)
        //    {
        //        foreach (TABS.CarrierAccount supplier in this.Suppliers)
        //        {
        //            data.Columns.Add(supplier.Name);
        //            if (supplier.Equals(nameSupplier))
        //                nameSupplierIndex = data.Columns.Count - 1;
        //            data.Columns.Add(supplier.Name + " Code");
        //        }
        //    }
        //    else
        //    {
        //        foreach (TABS.CarrierAccount supplier in this.Suppliers)
        //        {
        //            data.Columns.Add(supplier.Name);
        //            data.Columns.Add(supplier.Name + " Code");
        //        }
        //    }
        //    data.Columns.Add("SupplyCount");
        //    data.Columns.Add("MissingSupplyCount");
        //    data.Columns.Add("HasDifference");
        //    data.Columns.Add("IsNewCandidate");
        //    data.Columns.Add("IsDeleteCandidate");
        //    data.Columns.Add("Action");
        //    data.Columns.Add("Destination");


        //    foreach (TABS.Components.CodeComparison.ComparedCode comparedCode in this.Values.OrderBy(c => c.Code))
        //    {
        //        var systemIncreaseDays = (double)(decimal)TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveRateDays].Value;
        //        Code parentCode = null;
        //        if (!comparedCode.IsSold)
        //            parentCode = TABS.CodeMap.Current.Find(comparedCode.Code, CarrierAccount.SYSTEM, DateTime.Today.AddDays(systemIncreaseDays));

        //        DataRow row = data.NewRow();
        //        row["Code"] = comparedCode.Code;
        //        row["Group"] = comparedCode.CodeGroupName;
        //        row["Sale Zone"] = comparedCode.IsSold ? comparedCode.SaleZone.Name : (parentCode == null ? TABS.Zone.UndefinedZone.Name : parentCode.Zone.Name);
        //        row["Sale Code"] = comparedCode.IsSold ? comparedCode.SaleCode.Value : (parentCode == null ? string.Empty : parentCode.Value);
        //        if (comparedCode.IsNewCandidate) row["Action"] = "N";
        //        if (comparedCode.IsDeleteCandidate) row["Action"] = "D";
        //        foreach (TABS.CarrierAccount supplier in this.Suppliers)
        //        {
        //            row[supplier.Name] = comparedCode.GetZoneName(supplier);
        //            row[supplier.Name + " Code"] = comparedCode.GetCodeValue(supplier);
        //        }
        //        row["SupplyCount"] = comparedCode.SupplyCount;
        //        row["MissingSupplyCount"] = comparedCode.MissingSupplyCount;
        //        row["HasDifference"] = comparedCode.HasDifference ? "*" : string.Empty;
        //        row["IsNewCandidate"] = comparedCode.IsNewCandidate ? "*" : string.Empty;
        //        row["IsDeleteCandidate"] = comparedCode.IsDeleteCandidate ? "*" : string.Empty;
        //        row["Action"] = comparedCode.ActionFlag;
        //        if (comparedCode.IsNewCandidate && nameSupplierIndex > -1) row["Destination"] = row[nameSupplierIndex];
        //        if (comparedCode.IsDeleteCandidate) row["Destination"] = comparedCode.SaleZoneName;
        //        data.Rows.Add(row);
        //    }
        //    return data;
        //}

    }
}
