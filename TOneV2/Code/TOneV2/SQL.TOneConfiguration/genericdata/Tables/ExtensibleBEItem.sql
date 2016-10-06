CREATE TABLE [genericdata].[ExtensibleBEItem] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [OldID]       INT              NULL,
    [Details]     NVARCHAR (MAX)   NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_GenericEditorDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_GenericEditorDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);



