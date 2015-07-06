CREATE TABLE [dbo].[Strategy_Suspection_Level] (
    [Id]          INT IDENTITY (1, 1) NOT NULL,
    [StrategyId]  INT NULL,
    [LevelId]     INT NULL,
    [CriteriaId1] INT NULL,
    [CriteriaId2] INT NULL,
    [CriteriaId3] INT NULL,
    [CriteriaId4] INT NULL,
    [CriteriaId5] INT NULL,
    [CriteriaId6] INT NULL,
    CONSTRAINT [PK_Strategy_suspection_level] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Strategy_suspection_level_Strategy] FOREIGN KEY ([StrategyId]) REFERENCES [dbo].[Strategy] ([Id]),
    CONSTRAINT [FK_Strategy_suspection_level_suspection_level] FOREIGN KEY ([LevelId]) REFERENCES [dbo].[Suspection_Level] ([Id])
);

