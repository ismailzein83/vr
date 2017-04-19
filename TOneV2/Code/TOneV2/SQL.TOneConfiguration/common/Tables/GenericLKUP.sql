CREATE TABLE [common].[GenericLKUP] (
    [ID]                         UNIQUEIDENTIFIER NOT NULL,
    [Name]                       NVARCHAR (255)   NOT NULL,
    [BusinessEntityDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [Settings]                   NVARCHAR (MAX)   NULL,
    [CreatedTime]                DATETIME         CONSTRAINT [DF_GenericLKUP_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                  ROWVERSION       NULL,
    CONSTRAINT [PK_GenericLKUP] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_GenericLKUP_BEIdAndName]
    ON [common].[GenericLKUP]([BusinessEntityDefinitionID] ASC, [Name] ASC);

