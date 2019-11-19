CREATE TABLE [NIM].[ConnectionType] (
    [ID]                         UNIQUEIDENTIFIER NOT NULL,
    [Name]                       NVARCHAR (255)   NULL,
    [BusinessEntityDefinitionID] UNIQUEIDENTIFIER NULL,
    [CreatedTime]                DATETIME         NULL,
    [CreatedBy]                  INT              NULL,
    [LastModifiedTime]           DATETIME         NULL,
    [LastModifiedBy]             INT              NULL,
    [timestamp]                  ROWVERSION       NULL,
    [DevProjectID]               UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);



