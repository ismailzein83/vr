﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vanrise.Fzero.CDRAnalysis
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<CDR> CDRs { get; set; }
        public DbSet<ControlTable> ControlTables { get; set; }
        public DbSet<Criteria_Profile> Criteria_Profile { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<EmailReceiver> EmailReceivers { get; set; }
        public DbSet<EmailReceiverType> EmailReceiverTypes { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailToken> EmailTokens { get; set; }
        public DbSet<Import> Imports { get; set; }
        public DbSet<ImportType> ImportTypes { get; set; }
        public DbSet<MobileCDR> MobileCDRs { get; set; }
        public DbSet<NormalCDR> NormalCDRs { get; set; }
        public DbSet<NormalizationRule> NormalizationRules { get; set; }
        public DbSet<NormalizedCDR> NormalizedCDRs { get; set; }
        public DbSet<NumberProfile> NumberProfiles { get; set; }
        public DbSet<OperationType> OperationTypes { get; set; }
        public DbSet<Peak_Hoursold> Peak_Hoursold { get; set; }
        public DbSet<Peak_Time> Peak_Time { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PredefinedColumn> PredefinedColumns { get; set; }
        public DbSet<Related_Criteria> Related_Criteria { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportDetail> ReportDetails { get; set; }
        public DbSet<ReportingStatu> ReportingStatus { get; set; }
        public DbSet<SourceMapping> SourceMappings { get; set; }
        public DbSet<Strategy> Strategies { get; set; }
        public DbSet<Strategy_Min_Values> Strategy_Min_Values { get; set; }
        public DbSet<Strategy_Suspection_Level> Strategy_Suspection_Level { get; set; }
        public DbSet<StrategyPeriod> StrategyPeriods { get; set; }
        public DbSet<StrategyThreshold> StrategyThresholds { get; set; }
        public DbSet<Subscriber_Values> Subscriber_Values { get; set; }
        public DbSet<SubscriberThreshold> SubscriberThresholds { get; set; }
        public DbSet<Suspection_Level> Suspection_Level { get; set; }
        public DbSet<Switch_DatabaseConnections> Switch_DatabaseConnections { get; set; }
        public DbSet<SwitchProfile> SwitchProfiles { get; set; }
        public DbSet<SwitchTrunck> SwitchTruncks { get; set; }
        public DbSet<SwitchTruncksold> SwitchTruncksolds { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<Trunck> Truncks { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AN_Profiling> AN_Profiling { get; set; }
        public DbSet<NormalizationRulesBackup> NormalizationRulesBackups { get; set; }
        public DbSet<outgoing_t1> outgoing_t1 { get; set; }
        public DbSet<profiletest> profiletests { get; set; }
        public DbSet<vw_Dashboard> vw_Dashboard { get; set; }
        public DbSet<vw_ReportedNumber> vw_ReportedNumber { get; set; }
        public DbSet<vw_ReportedNumberNormalCDR> vw_ReportedNumberNormalCDR { get; set; }
        public DbSet<vwSuspectionAnalysi> vwSuspectionAnalysis { get; set; }
    
        public virtual ObjectResult<FindSuspectionOccurance_Result> FindSuspectionOccurance(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, Nullable<int> strategyId, string suspectionList, Nullable<int> minimumOccurance)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("fromDate", fromDate) :
                new ObjectParameter("fromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            var strategyIdParameter = strategyId.HasValue ?
                new ObjectParameter("strategyId", strategyId) :
                new ObjectParameter("strategyId", typeof(int));
    
            var suspectionListParameter = suspectionList != null ?
                new ObjectParameter("SuspectionList", suspectionList) :
                new ObjectParameter("SuspectionList", typeof(string));
    
            var minimumOccuranceParameter = minimumOccurance.HasValue ?
                new ObjectParameter("MinimumOccurance", minimumOccurance) :
                new ObjectParameter("MinimumOccurance", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<FindSuspectionOccurance_Result>("FindSuspectionOccurance", fromDateParameter, toDateParameter, strategyIdParameter, suspectionListParameter, minimumOccuranceParameter);
        }
    
        public virtual ObjectResult<db_GetReportVariables_Result> GetReportVariables()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<db_GetReportVariables_Result>("GetReportVariables");
        }
    
        public virtual int db_Collect_All_Data()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_Collect_All_Data");
        }
    
        public virtual int db_collect_data(Nullable<int> switchId)
        {
            var switchIdParameter = switchId.HasValue ?
                new ObjectParameter("SwitchId", switchId) :
                new ObjectParameter("SwitchId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_collect_data", switchIdParameter);
        }
    
        public virtual int db_fill_In_trunkType()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_fill_In_trunkType");
        }
    
        public virtual int db_fill_out_trunkType()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_fill_out_trunkType");
        }
    
        public virtual int db_FillSubscriberThresholds(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, Nullable<int> strategyId)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            var strategyIdParameter = strategyId.HasValue ?
                new ObjectParameter("StrategyId", strategyId) :
                new ObjectParameter("StrategyId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_FillSubscriberThresholds", fromDateParameter, toDateParameter, strategyIdParameter);
        }
    
        public virtual int db_FillSubscriberValues(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, Nullable<int> strategyId)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            var strategyIdParameter = strategyId.HasValue ?
                new ObjectParameter("StrategyId", strategyId) :
                new ObjectParameter("StrategyId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_FillSubscriberValues", fromDateParameter, toDateParameter, strategyIdParameter);
        }
    
        public virtual int db_FillSwitchTrunks()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_FillSwitchTrunks");
        }
    
        public virtual ObjectResult<db_findSuspectionOccuranceOld_Result> db_findSuspectionOccuranceOld(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, Nullable<int> strategyId)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("fromDate", fromDate) :
                new ObjectParameter("fromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            var strategyIdParameter = strategyId.HasValue ?
                new ObjectParameter("strategyId", strategyId) :
                new ObjectParameter("strategyId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<db_findSuspectionOccuranceOld_Result>("db_findSuspectionOccuranceOld", fromDateParameter, toDateParameter, strategyIdParameter);
        }
    
        public virtual int db_GetUnNormalizedCDPN(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, string database)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            var databaseParameter = database != null ?
                new ObjectParameter("Database", database) :
                new ObjectParameter("Database", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_GetUnNormalizedCDPN", fromDateParameter, toDateParameter, databaseParameter);
        }
    
        public virtual int db_GetUnNormalizedCGPN(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, string database)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            var databaseParameter = database != null ?
                new ObjectParameter("Database", database) :
                new ObjectParameter("Database", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_GetUnNormalizedCGPN", fromDateParameter, toDateParameter, databaseParameter);
        }
    
        public virtual int db_normalization(Nullable<int> switchId)
        {
            var switchIdParameter = switchId.HasValue ?
                new ObjectParameter("SwitchId", switchId) :
                new ObjectParameter("SwitchId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_normalization", switchIdParameter);
        }
    
        public virtual int db_profile()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_profile");
        }
    
        public virtual int db_profileold()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_profileold");
        }
    
        public virtual int db_UpdateRepeatedCalls()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_UpdateRepeatedCalls");
        }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    
        public virtual int db_Dashboard(string database)
        {
            var databaseParameter = database != null ?
                new ObjectParameter("Database", database) :
                new ObjectParameter("Database", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_Dashboard", databaseParameter);
        }
    
        public virtual ObjectResult<db_GetReportedNumberNormalCDR_Result> db_GetReportedNumberNormalCDR(Nullable<int> reportID)
        {
            var reportIDParameter = reportID.HasValue ?
                new ObjectParameter("ReportID", reportID) :
                new ObjectParameter("ReportID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<db_GetReportedNumberNormalCDR_Result>("db_GetReportedNumberNormalCDR", reportIDParameter);
        }
    
        public virtual ObjectResult<prGetEmails_Result> prGetEmails(string destinationEmail, Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate)
        {
            var destinationEmailParameter = destinationEmail != null ?
                new ObjectParameter("DestinationEmail", destinationEmail) :
                new ObjectParameter("DestinationEmail", typeof(string));
    
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<prGetEmails_Result>("prGetEmails", destinationEmailParameter, fromDateParameter, toDateParameter);
        }
    
        public virtual int prDeleteEmail(string ids)
        {
            var idsParameter = ids != null ?
                new ObjectParameter("Ids", ids) :
                new ObjectParameter("Ids", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prDeleteEmail", idsParameter);
        }
    
        public virtual int prSendMail(string profile_name, string recipients, string subject, string copy_recipients, string body, string blind_copy_recipients)
        {
            var profile_nameParameter = profile_name != null ?
                new ObjectParameter("profile_name", profile_name) :
                new ObjectParameter("profile_name", typeof(string));
    
            var recipientsParameter = recipients != null ?
                new ObjectParameter("recipients", recipients) :
                new ObjectParameter("recipients", typeof(string));
    
            var subjectParameter = subject != null ?
                new ObjectParameter("subject", subject) :
                new ObjectParameter("subject", typeof(string));
    
            var copy_recipientsParameter = copy_recipients != null ?
                new ObjectParameter("copy_recipients", copy_recipients) :
                new ObjectParameter("copy_recipients", typeof(string));
    
            var bodyParameter = body != null ?
                new ObjectParameter("body", body) :
                new ObjectParameter("body", typeof(string));
    
            var blind_copy_recipientsParameter = blind_copy_recipients != null ?
                new ObjectParameter("blind_copy_recipients", blind_copy_recipients) :
                new ObjectParameter("blind_copy_recipients", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prSendMail", profile_nameParameter, recipientsParameter, subjectParameter, copy_recipientsParameter, bodyParameter, blind_copy_recipientsParameter);
        }
    
        public virtual int prSendMailWithAttachment(string profile_name, string recipients, string subject, string copy_recipients, string body, string file_attachments, string blind_copy_recipients)
        {
            var profile_nameParameter = profile_name != null ?
                new ObjectParameter("profile_name", profile_name) :
                new ObjectParameter("profile_name", typeof(string));
    
            var recipientsParameter = recipients != null ?
                new ObjectParameter("recipients", recipients) :
                new ObjectParameter("recipients", typeof(string));
    
            var subjectParameter = subject != null ?
                new ObjectParameter("subject", subject) :
                new ObjectParameter("subject", typeof(string));
    
            var copy_recipientsParameter = copy_recipients != null ?
                new ObjectParameter("copy_recipients", copy_recipients) :
                new ObjectParameter("copy_recipients", typeof(string));
    
            var bodyParameter = body != null ?
                new ObjectParameter("body", body) :
                new ObjectParameter("body", typeof(string));
    
            var file_attachmentsParameter = file_attachments != null ?
                new ObjectParameter("file_attachments", file_attachments) :
                new ObjectParameter("file_attachments", typeof(string));
    
            var blind_copy_recipientsParameter = blind_copy_recipients != null ?
                new ObjectParameter("blind_copy_recipients", blind_copy_recipients) :
                new ObjectParameter("blind_copy_recipients", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prSendMailWithAttachment", profile_nameParameter, recipientsParameter, subjectParameter, copy_recipientsParameter, bodyParameter, file_attachmentsParameter, blind_copy_recipientsParameter);
        }
    
        public virtual int SaveMail(string profile_name, string recipients, string subject, string body, string copy_recipients, Nullable<int> emailTemplateId)
        {
            var profile_nameParameter = profile_name != null ?
                new ObjectParameter("profile_name", profile_name) :
                new ObjectParameter("profile_name", typeof(string));
    
            var recipientsParameter = recipients != null ?
                new ObjectParameter("recipients", recipients) :
                new ObjectParameter("recipients", typeof(string));
    
            var subjectParameter = subject != null ?
                new ObjectParameter("subject", subject) :
                new ObjectParameter("subject", typeof(string));
    
            var bodyParameter = body != null ?
                new ObjectParameter("body", body) :
                new ObjectParameter("body", typeof(string));
    
            var copy_recipientsParameter = copy_recipients != null ?
                new ObjectParameter("copy_recipients", copy_recipients) :
                new ObjectParameter("copy_recipients", typeof(string));
    
            var emailTemplateIdParameter = emailTemplateId.HasValue ?
                new ObjectParameter("EmailTemplateId", emailTemplateId) :
                new ObjectParameter("EmailTemplateId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SaveMail", profile_nameParameter, recipientsParameter, subjectParameter, bodyParameter, copy_recipientsParameter, emailTemplateIdParameter);
        }
    
        public virtual int SaveMailWithAttachement(string profile_name, string recipients, string subject, string body, Nullable<int> emailTemplateId, string copy_recipients, string file_attachments)
        {
            var profile_nameParameter = profile_name != null ?
                new ObjectParameter("profile_name", profile_name) :
                new ObjectParameter("profile_name", typeof(string));
    
            var recipientsParameter = recipients != null ?
                new ObjectParameter("recipients", recipients) :
                new ObjectParameter("recipients", typeof(string));
    
            var subjectParameter = subject != null ?
                new ObjectParameter("subject", subject) :
                new ObjectParameter("subject", typeof(string));
    
            var bodyParameter = body != null ?
                new ObjectParameter("body", body) :
                new ObjectParameter("body", typeof(string));
    
            var emailTemplateIdParameter = emailTemplateId.HasValue ?
                new ObjectParameter("EmailTemplateId", emailTemplateId) :
                new ObjectParameter("EmailTemplateId", typeof(int));
    
            var copy_recipientsParameter = copy_recipients != null ?
                new ObjectParameter("copy_recipients", copy_recipients) :
                new ObjectParameter("copy_recipients", typeof(string));
    
            var file_attachmentsParameter = file_attachments != null ?
                new ObjectParameter("file_attachments", file_attachments) :
                new ObjectParameter("file_attachments", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SaveMailWithAttachement", profile_nameParameter, recipientsParameter, subjectParameter, bodyParameter, emailTemplateIdParameter, copy_recipientsParameter, file_attachmentsParameter);
        }
    
        public virtual int prfindSuspectionOccurance(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, Nullable<int> strategyId, string suspectionList, Nullable<int> minimumOccurance)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("fromDate", fromDate) :
                new ObjectParameter("fromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            var strategyIdParameter = strategyId.HasValue ?
                new ObjectParameter("strategyId", strategyId) :
                new ObjectParameter("strategyId", typeof(int));
    
            var suspectionListParameter = suspectionList != null ?
                new ObjectParameter("SuspectionList", suspectionList) :
                new ObjectParameter("SuspectionList", typeof(string));
    
            var minimumOccuranceParameter = minimumOccurance.HasValue ?
                new ObjectParameter("MinimumOccurance", minimumOccurance) :
                new ObjectParameter("MinimumOccurance", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prfindSuspectionOccurance", fromDateParameter, toDateParameter, strategyIdParameter, suspectionListParameter, minimumOccuranceParameter);
        }
    
        public virtual int delete_db_findSuspectionOccurance(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, Nullable<int> strategyId, string suspectionList, Nullable<int> minimumOccurance)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("fromDate", fromDate) :
                new ObjectParameter("fromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            var strategyIdParameter = strategyId.HasValue ?
                new ObjectParameter("strategyId", strategyId) :
                new ObjectParameter("strategyId", typeof(int));
    
            var suspectionListParameter = suspectionList != null ?
                new ObjectParameter("SuspectionList", suspectionList) :
                new ObjectParameter("SuspectionList", typeof(string));
    
            var minimumOccuranceParameter = minimumOccurance.HasValue ?
                new ObjectParameter("MinimumOccurance", minimumOccurance) :
                new ObjectParameter("MinimumOccurance", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("delete_db_findSuspectionOccurance", fromDateParameter, toDateParameter, strategyIdParameter, suspectionListParameter, minimumOccuranceParameter);
        }
    
        public virtual int db_findSuspectionOccurance(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, Nullable<int> strategyId, string suspectionList, Nullable<int> minimumOccurance)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("fromDate", fromDate) :
                new ObjectParameter("fromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            var strategyIdParameter = strategyId.HasValue ?
                new ObjectParameter("strategyId", strategyId) :
                new ObjectParameter("strategyId", typeof(int));
    
            var suspectionListParameter = suspectionList != null ?
                new ObjectParameter("SuspectionList", suspectionList) :
                new ObjectParameter("SuspectionList", typeof(string));
    
            var minimumOccuranceParameter = minimumOccurance.HasValue ?
                new ObjectParameter("MinimumOccurance", minimumOccurance) :
                new ObjectParameter("MinimumOccurance", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("db_findSuspectionOccurance", fromDateParameter, toDateParameter, strategyIdParameter, suspectionListParameter, minimumOccuranceParameter);
        }
    
        public virtual int prImportfromSwitch(Nullable<int> switchID, string dBName, Nullable<int> reference)
        {
            var switchIDParameter = switchID.HasValue ?
                new ObjectParameter("SwitchID", switchID) :
                new ObjectParameter("SwitchID", typeof(int));
    
            var dBNameParameter = dBName != null ?
                new ObjectParameter("DBName", dBName) :
                new ObjectParameter("DBName", typeof(string));
    
            var referenceParameter = reference.HasValue ?
                new ObjectParameter("Reference", reference) :
                new ObjectParameter("Reference", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prImportfromSwitch", switchIDParameter, dBNameParameter, referenceParameter);
        }
    
        public virtual ObjectResult<prImportfromSwitches_Result> prImportfromSwitches()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<prImportfromSwitches_Result>("prImportfromSwitches");
        }
    
        public virtual int prCollectNormalizedCDR()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prCollectNormalizedCDR");
        }
    
        public virtual int prFillSubscriberThresholds(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, Nullable<int> strategyId)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            var strategyIdParameter = strategyId.HasValue ?
                new ObjectParameter("StrategyId", strategyId) :
                new ObjectParameter("StrategyId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prFillSubscriberThresholds", fromDateParameter, toDateParameter, strategyIdParameter);
        }
    
        public virtual int prFillSubscriberValues(Nullable<System.DateTime> fromDate, Nullable<System.DateTime> toDate, Nullable<int> strategyId)
        {
            var fromDateParameter = fromDate.HasValue ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(System.DateTime));
    
            var toDateParameter = toDate.HasValue ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(System.DateTime));
    
            var strategyIdParameter = strategyId.HasValue ?
                new ObjectParameter("StrategyId", strategyId) :
                new ObjectParameter("StrategyId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prFillSubscriberValues", fromDateParameter, toDateParameter, strategyIdParameter);
        }
    
        public virtual int prNormalization()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prNormalization");
        }
    
        public virtual int prNumberProfiling()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prNumberProfiling");
        }
    
        public virtual int prOldUpdateRepeatedCalls()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prOldUpdateRepeatedCalls");
        }
    
        public virtual int prUpdateRepeatedCalls()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("prUpdateRepeatedCalls");
        }
    }
}
