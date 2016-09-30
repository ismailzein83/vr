CREATE TABLE [genericdata].[GenericBusinessEntity] (
    [ID]                            BIGINT           IDENTITY (1, 1) NOT NULL,
    [BusinessEntityDefinitionID]    UNIQUEIDENTIFIER NULL,
    [OldBusinessEntityDefinitionID] INT              NULL,
    [Details]                       NVARCHAR (MAX)   NULL,
    [CreatedTime]                   DATETIME         CONSTRAINT [DF_GenericBusinessEntity_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                     ROWVERSION       NULL,
    CONSTRAINT [PK_GenericBusinessEntity] PRIMARY KEY CLUSTERED ([ID] ASC)
);



