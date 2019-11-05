CREATE TABLE [RecordAnalysis].[SuspiciousNumber] (
    [Id]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [Number]           NVARCHAR (255)   NULL,
    [RuleId]           BIGINT           NULL,
    [Status]           UNIQUEIDENTIFIER NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    CONSTRAINT [PK__Suspicio__3214EC0700551192] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_SuspiciousNumber_CreatedTime]
    ON [RecordAnalysis].[SuspiciousNumber]([CreatedTime] ASC);

