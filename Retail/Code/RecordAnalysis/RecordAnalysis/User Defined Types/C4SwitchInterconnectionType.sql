CREATE TYPE [RecordAnalysis].[C4SwitchInterconnectionType] AS TABLE (
    [Id]                INT            NULL,
    [SwitchId]          INT            NULL,
    [InterconnectionId] INT            NULL,
    [Settings]          NVARCHAR (MAX) NULL,
    [CreatedBy]         INT            NULL,
    [CreatedTime]       DATETIME       NULL,
    [LastModifiedBy]    INT            NULL,
    [LastModifiedTime]  DATETIME       NULL);

