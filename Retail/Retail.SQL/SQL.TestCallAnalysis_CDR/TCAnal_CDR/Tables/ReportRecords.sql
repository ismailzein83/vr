CREATE TABLE [TCAnal_CDR].[ReportRecords] (
    [Id]               BIGINT   IDENTITY (1, 1) NOT NULL,
    [ReportId]         INT      NULL,
    [CaseId]           BIGINT   NULL,
    [CreatedTime]      DATETIME NULL,
    [CreatedBy]        INT      NULL,
    [LastModifiedTime] DATETIME NULL,
    [LastModifiedBy]   INT      NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

