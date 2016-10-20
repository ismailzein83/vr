CREATE TABLE [bp].[BPBusinessRuleDefinition] (
    [ID]               UNIQUEIDENTIFIER CONSTRAINT [DF__BPBusinessRu__ID__12F3B011] DEFAULT (newid()) NOT NULL,
    [Name]             VARCHAR (255)    NOT NULL,
    [BPDefintionId]    UNIQUEIDENTIFIER NULL,
    [OldBPDefintionId] INT              NOT NULL,
    [Settings]         NVARCHAR (MAX)   NOT NULL,
    [timestamp]        ROWVERSION       NOT NULL,
    [OldID]            INT              NULL,
    CONSTRAINT [pk_BPBusinessRuleDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);



