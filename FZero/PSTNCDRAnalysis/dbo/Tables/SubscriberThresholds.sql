CREATE TABLE [dbo].[SubscriberThresholds] (
    [Id]                INT          IDENTITY (1, 1) NOT NULL,
    [DateDay]           DATETIME     NULL,
    [SubscriberNumber]  VARCHAR (50) NULL,
    [Criteria1]         INT          NULL,
    [Criteria2]         INT          NULL,
    [Criteria3]         INT          NULL,
    [Criteria4]         INT          NULL,
    [Criteria5]         INT          NULL,
    [Criteria6]         INT          NULL,
    [SuspectionLevelId] INT          NULL,
    [StrategyId]        INT          NULL,
    CONSTRAINT [PK_SubscriberThresholds] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SubscriberThresholds_Suspection_Level] FOREIGN KEY ([SuspectionLevelId]) REFERENCES [dbo].[Suspection_Level] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Strategy]
    ON [dbo].[SubscriberThresholds]([SuspectionLevelId] ASC, [StrategyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateDay]
    ON [dbo].[SubscriberThresholds]([DateDay] ASC);

