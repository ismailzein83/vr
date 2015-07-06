CREATE TABLE [dbo].[StrategyThreshold] (
    [Id]         INT             IDENTITY (1, 1) NOT NULL,
    [StrategyId] INT             NULL,
    [MaxValue]   DECIMAL (18, 3) NULL,
    [CriteriaID] INT             NULL,
    CONSTRAINT [PK_StrategyThreshold] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StrategyThreshold_criteria_profile] FOREIGN KEY ([CriteriaID]) REFERENCES [dbo].[Criteria_Profile] ([Id]),
    CONSTRAINT [FK_StrategyThreshold_Strategy] FOREIGN KEY ([StrategyId]) REFERENCES [dbo].[Strategy] ([Id])
);

