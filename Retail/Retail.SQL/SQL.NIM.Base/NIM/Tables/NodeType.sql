CREATE TABLE [NIM].[NodeType] (
    [ID]                         UNIQUEIDENTIFIER NULL,
    [Name]                       NVARCHAR (255)   NULL,
    [BusinessEntityDefinitionID] UNIQUEIDENTIFIER NULL,
    [CreatedTime]                DATETIME         NULL,
    [CreatedBy]                  INT              NULL,
    [LastModifiedTime]           DATETIME         NULL,
    [LastModifiedBy]             INT              NULL,
    [timestamp]                  ROWVERSION       NULL
);

