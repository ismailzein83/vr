CREATE TABLE [dbo].[StrategyPeriods] (
    [Id]         INT IDENTITY (1, 1) NOT NULL,
    [StrategyId] INT NULL,
    [PeriodId]   INT NULL,
    [Value]      INT NULL,
    [CriteriaID] INT NULL,
    CONSTRAINT [PK_StrategyPeriods] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StrategyPeriods_criteria_profile] FOREIGN KEY ([CriteriaID]) REFERENCES [dbo].[Criteria_Profile] ([Id]),
    CONSTRAINT [FK_StrategyPeriods_Periods] FOREIGN KEY ([PeriodId]) REFERENCES [dbo].[Periods] ([Id]),
    CONSTRAINT [FK_StrategyPeriods_Strategy] FOREIGN KEY ([StrategyId]) REFERENCES [dbo].[Strategy] ([Id])
);

