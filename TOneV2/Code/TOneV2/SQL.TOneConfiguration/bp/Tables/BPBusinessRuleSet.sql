CREATE TABLE [bp].[BPBusinessRuleSet] (
    [ID]               INT              IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (MAX)   NOT NULL,
    [ParentID]         INT              NULL,
    [Details]          NVARCHAR (MAX)   NULL,
    [BPDefinitionId]   UNIQUEIDENTIFIER NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_BPBusinessRuleSet_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_BPBusinessRuleSet_LastModifiedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NOT NULL,
    CONSTRAINT [PK_BPBusinessRuleSet] PRIMARY KEY CLUSTERED ([ID] ASC)
);













