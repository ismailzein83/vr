CREATE TABLE [LCR].[RoutingRuleAction] (
    [ID]       INT            IDENTITY (1, 1) NOT NULL,
    [Type]     INT            NOT NULL,
    [FQTN]     VARCHAR (1000) NOT NULL,
    [Priority] INT            NOT NULL,
    CONSTRAINT [PK_RoutingRuleAction] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_Table_Priority] UNIQUE NONCLUSTERED ([Priority] ASC)
);

