CREATE TABLE [bp].[BPBusinessRuleAction] (
    [ID]                          INT              IDENTITY (1, 1) NOT NULL,
    [Settings]                    NVARCHAR (MAX)   NOT NULL,
    [timestamp]                   ROWVERSION       NOT NULL,
    [OldBusinessRuleDefinitionId] INT              NULL,
    [BusinessRuleDefinitionId]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_BusinessRuleAction] PRIMARY KEY CLUSTERED ([ID] ASC)
);



