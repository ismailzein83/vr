CREATE TABLE [rules].[RuleType] (
    [ID]   INT           IDENTITY (1, 1) NOT NULL,
    [Type] VARCHAR (255) NOT NULL,
    CONSTRAINT [PK_RuleType] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_RuleType_Type] UNIQUE NONCLUSTERED ([Type] ASC)
);

