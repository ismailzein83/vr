CREATE TABLE [dbo].[Related_Criteria] (
    [Id]                 INT IDENTITY (1, 1) NOT NULL,
    [StrategyId]         INT NULL,
    [CriteriaId]         INT NULL,
    [Related_CriteriaId] INT NULL,
    CONSTRAINT [PK_Related_Criteria] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Related_Criteria_criteria_profile] FOREIGN KEY ([CriteriaId]) REFERENCES [dbo].[Criteria_Profile] ([Id]),
    CONSTRAINT [FK_Related_Criteria_criteria_profile1] FOREIGN KEY ([Related_CriteriaId]) REFERENCES [dbo].[Criteria_Profile] ([Id]),
    CONSTRAINT [FK_Related_Criteria_Strategy] FOREIGN KEY ([StrategyId]) REFERENCES [dbo].[Strategy] ([Id])
);

