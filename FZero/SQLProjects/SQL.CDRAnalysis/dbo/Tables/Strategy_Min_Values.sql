CREATE TABLE [dbo].[Strategy_Min_Values] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [StrategyId]        INT             NULL,
    [Min_Count_Value]   INT             NULL,
    [Min_Aggreg_Volume] NUMERIC (13, 4) NULL,
    [CriteriaId]        INT             NULL,
    CONSTRAINT [PK_Strategy_Min_Values] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Strategy_Min_Values_criteria_profile] FOREIGN KEY ([CriteriaId]) REFERENCES [dbo].[Criteria_Profile] ([Id]),
    CONSTRAINT [FK_Strategy_Min_Values_Strategy] FOREIGN KEY ([StrategyId]) REFERENCES [dbo].[Strategy] ([Id])
);

