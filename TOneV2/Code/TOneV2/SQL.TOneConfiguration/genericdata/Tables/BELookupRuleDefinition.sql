CREATE TABLE [genericdata].[BELookupRuleDefinition] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (450)   NOT NULL,
    [Details]          NVARCHAR (MAX)   NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_BELookupRuleDefinition_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_BELookupRuleDefinition_LastModifiedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_BELookupRuleDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_BELookupRuleDefinition_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);







