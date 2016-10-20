CREATE TABLE [bp].[BPBusinessRuleSet] (
    [ID]                INT              IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (MAX)   NOT NULL,
    [ParentID]          INT              NULL,
    [Details]           NVARCHAR (MAX)   NULL,
    [timestamp]         ROWVERSION       NOT NULL,
    [OldBPDefinitionId] INT              NULL,
    [BPDefinitionId]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_BPBusinessRuleSet] PRIMARY KEY CLUSTERED ([ID] ASC)
);





