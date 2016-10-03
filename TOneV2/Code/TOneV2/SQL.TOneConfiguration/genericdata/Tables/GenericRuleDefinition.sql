CREATE TABLE [genericdata].[GenericRuleDefinition] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [OldID]       INT              NULL,
    [Name]        NVARCHAR (900)   NOT NULL,
    [Details]     NVARCHAR (MAX)   NOT NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_GenericRuleDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_GenericRuleDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);



