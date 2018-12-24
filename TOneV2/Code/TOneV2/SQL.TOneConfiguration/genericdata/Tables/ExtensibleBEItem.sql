CREATE TABLE [genericdata].[ExtensibleBEItem] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Details]          NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_GenericEditorDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_ExtensibleBEItem_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_GenericEditorDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);







