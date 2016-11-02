CREATE TABLE [bp].[BPBusinessRuleAction] (
    [ID]                          INT              IDENTITY (1, 1) NOT NULL,
    [Settings]                    NVARCHAR (MAX)   NOT NULL,
    [BusinessRuleDefinitionId]    UNIQUEIDENTIFIER NOT NULL,
    [OldBusinessRuleDefinitionId] INT              NULL,
    [timestamp]                   ROWVERSION       NOT NULL,
    CONSTRAINT [PK_BusinessRuleAction] PRIMARY KEY CLUSTERED ([ID] ASC)
);





