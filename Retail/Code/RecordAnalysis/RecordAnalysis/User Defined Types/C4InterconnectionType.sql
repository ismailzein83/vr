CREATE TYPE [RecordAnalysis].[C4InterconnectionType] AS TABLE (
    [Id]               INT           NULL,
    [Name]             VARCHAR (255) NULL,
    [CreatedBy]        INT           NULL,
    [CreatedTime]      DATETIME      NULL,
    [LastModifiedBy]   INT           NULL,
    [LastModifiedTime] DATETIME      NULL);

