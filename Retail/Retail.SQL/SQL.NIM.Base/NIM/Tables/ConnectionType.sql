CREATE TABLE [NIM].[ConnectionType] (
    [ID]                         UNIQUEIDENTIFIER NOT NULL,
    [DevProjectID]               UNIQUEIDENTIFIER NULL,
    [Name]                       NVARCHAR (255)   NULL,
    [BusinessEntityDefinitionID] UNIQUEIDENTIFIER NULL,
    [CreatedTime]                DATETIME         NULL,
    [CreatedBy]                  INT              NULL,
    [LastModifiedTime]           DATETIME         NULL,
    [LastModifiedBy]             INT              NULL,
    [timestamp]                  ROWVERSION       NULL,
    CONSTRAINT [PK__Connecti__3214EC27184C96B4] PRIMARY KEY CLUSTERED ([ID] ASC)
);





