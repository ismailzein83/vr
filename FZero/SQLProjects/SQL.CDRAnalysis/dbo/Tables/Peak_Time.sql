CREATE TABLE [dbo].[Peak_Time] (
    [Id]         INT IDENTITY (1, 1) NOT NULL,
    [Peak_Hour]  INT NOT NULL,
    [StrategyId] INT NULL,
    CONSTRAINT [PK_Peak_time] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Peak_time_Strategy] FOREIGN KEY ([StrategyId]) REFERENCES [dbo].[Strategy] ([Id])
);

