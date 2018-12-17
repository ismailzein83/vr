CREATE TABLE [bp].[BPBusinessRuleAction] (
    [ID]                       INT              IDENTITY (1, 1) NOT NULL,
    [Settings]                 NVARCHAR (MAX)   NOT NULL,
    [BusinessRuleDefinitionId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedTime]              DATETIME         CONSTRAINT [DF_BPBusinessRuleAction_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime]         DATETIME         CONSTRAINT [DF_BPBusinessRuleAction_LastModifiedTime] DEFAULT (getdate()) NULL,
    [timestamp]                ROWVERSION       NOT NULL,
    CONSTRAINT [PK_BusinessRuleAction] PRIMARY KEY CLUSTERED ([ID] ASC)
);









