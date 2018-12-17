CREATE TABLE [bp].[BPBusinessRuleDefinition] (
    [ID]               UNIQUEIDENTIFIER CONSTRAINT [DF__BPBusinessRu__ID__12F3B011] DEFAULT (newid()) NOT NULL,
    [Name]             VARCHAR (255)    NOT NULL,
    [BPDefintionId]    UNIQUEIDENTIFIER NOT NULL,
    [Settings]         NVARCHAR (MAX)   NOT NULL,
    [Rank]             INT              NOT NULL,
    [LastModifiedTime] DATETIME         CONSTRAINT [DF_BPBusinessRuleDefinition_LastModifiedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION       NOT NULL,
    CONSTRAINT [pk_BPBusinessRuleDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);











