CREATE TABLE [common].[StatusDefinition] (
    [ID]                         UNIQUEIDENTIFIER NOT NULL,
    [Name]                       NVARCHAR (255)   NOT NULL,
    [BusinessEntityDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [Settings]                   NVARCHAR (MAX)   NULL,
    [CreatedTime]                DATETIME         CONSTRAINT [DF_StatusDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                  ROWVERSION       NULL,
    [CreatedBy]                  INT              NULL,
    [LastModifiedBy]             INT              NULL,
    [LastModifiedTime]           DATETIME         NULL,
    CONSTRAINT [PK_StatusDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);



