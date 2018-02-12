CREATE TABLE [TOneWhS_Case].[CaseWorkGroup] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [CreatedTime]      DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    CONSTRAINT [PK_CustomerCaseWorkGroup] PRIMARY KEY CLUSTERED ([ID] ASC)
);



