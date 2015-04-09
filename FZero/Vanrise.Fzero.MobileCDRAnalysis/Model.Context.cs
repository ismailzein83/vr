﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vanrise.Fzero.MobileCDRAnalysis
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MobileEntities : DbContext
    {
        public MobileEntities()
            : base("name=MobileEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<CDR> CDRs { get; set; }
        public DbSet<CDR_Old> CDR_Old { get; set; }
        public DbSet<CellProfile> CellProfiles { get; set; }
        public DbSet<ControlTable> ControlTables { get; set; }
        public DbSet<Criteria_Profile> Criteria_Profile { get; set; }
        public DbSet<delete_NumberProfile> delete_NumberProfile { get; set; }
        public DbSet<delete_Peak_Hours> delete_Peak_Hours { get; set; }
        public DbSet<EmailReceiver> EmailReceivers { get; set; }
        public DbSet<EmailReceiverType> EmailReceiverTypes { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailToken> EmailTokens { get; set; }
        public DbSet<Import> Imports { get; set; }
        public DbSet<ImportType> ImportTypes { get; set; }
        public DbSet<NormalCDR> NormalCDRs { get; set; }
        public DbSet<NormalCDR_Old> NormalCDR_Old { get; set; }
        public DbSet<NormalCDR_ToDelete> NormalCDR_ToDelete { get; set; }
        public DbSet<NormalizationRule> NormalizationRules { get; set; }
        public DbSet<NumberProfile> NumberProfiles { get; set; }
        public DbSet<OperationType> OperationTypes { get; set; }
        public DbSet<Peak_Time> Peak_Time { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PredefinedColumn> PredefinedColumns { get; set; }
        public DbSet<Related_Criteria> Related_Criteria { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportDetail> ReportDetails { get; set; }
        public DbSet<ReportingStatu> ReportingStatus { get; set; }
        public DbSet<Set_CallClass> Set_CallClass { get; set; }
        public DbSet<Set_CallType> Set_CallType { get; set; }
        public DbSet<Set_SubType> Set_SubType { get; set; }
        public DbSet<SourceMapping> SourceMappings { get; set; }
        public DbSet<Strategy> Strategies { get; set; }
        public DbSet<Strategy_Min_Values> Strategy_Min_Values { get; set; }
        public DbSet<Strategy_Suspicion_Level> Strategy_Suspicion_Level { get; set; }
        public DbSet<StrategyPeriod> StrategyPeriods { get; set; }
        public DbSet<StrategyThreshold> StrategyThresholds { get; set; }
        public DbSet<Subscriber_Values> Subscriber_Values { get; set; }
        public DbSet<Subscriber_ValuesNew> Subscriber_ValuesNew { get; set; }
        public DbSet<SubscriberThreshold> SubscriberThresholds { get; set; }
        public DbSet<SubscriberThresholdstest> SubscriberThresholdstests { get; set; }
        public DbSet<Suspicion_Level> Suspicion_Level { get; set; }
        public DbSet<Switch_DatabaseConnections> Switch_DatabaseConnections { get; set; }
        public DbSet<SwitchProfile> SwitchProfiles { get; set; }
        public DbSet<sysdiagram> sysdiagrams { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<vwDashboard> vwDashboards { get; set; }
        public DbSet<vwReportedNumber> vwReportedNumbers { get; set; }
        public DbSet<vwReportedNumberNormalCDR> vwReportedNumberNormalCDRs { get; set; }
        public DbSet<vwReportVariable> vwReportVariables { get; set; }
        public DbSet<vwSuspectionAnalysi> vwSuspectionAnalysis { get; set; }
    }
}
