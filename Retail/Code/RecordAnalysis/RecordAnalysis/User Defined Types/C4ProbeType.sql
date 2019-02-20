CREATE TYPE [RecordAnalysis].[C4ProbeType] AS TABLE (
    [Id]               INT            NULL,
    [Name]             VARCHAR (255)  NULL,
    [Settings]         NVARCHAR (MAX) NULL,
    [CreatedBy]        INT            NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL);

