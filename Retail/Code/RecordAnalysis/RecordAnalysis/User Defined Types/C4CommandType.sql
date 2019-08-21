CREATE TYPE [RecordAnalysis].[C4CommandType] AS TABLE (
    [Id]          INT            NULL,
    [Type]        INT            NULL,
    [Command]     NVARCHAR (255) NULL,
    [CreatedTime] DATETIME       NULL);

