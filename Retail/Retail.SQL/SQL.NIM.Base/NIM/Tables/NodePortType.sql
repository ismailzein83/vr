CREATE TABLE [NIM].[NodePortType] (
    [ID]                         UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]               UNIQUEIDENTIFIER NULL,
    [Name]                       NVARCHAR (255)   NULL,
    [BusinessEntityDefinitionID] UNIQUEIDENTIFIER NULL,
    [CreatedTime]                DATETIME         NULL,
    [CreatedBy]                  INT              NULL,
    [LastModifiedTime]           DATETIME         NULL,
    [LastModifiedBy]             INT              NULL,
    [timestamp]                  ROWVERSION       NULL,
    CONSTRAINT [PK_NodePortType] PRIMARY KEY CLUSTERED ([ID] ASC)
);





