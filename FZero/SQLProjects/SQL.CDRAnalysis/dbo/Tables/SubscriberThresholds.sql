CREATE TABLE [dbo].[SubscriberThresholds] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [DateDay]           DATETIME        NULL,
    [SubscriberNumber]  VARCHAR (50)    NULL,
    [Criteria1]         DECIMAL (18, 2) NULL,
    [Criteria2]         DECIMAL (18, 2) NULL,
    [Criteria3]         DECIMAL (18, 2) NULL,
    [Criteria4]         DECIMAL (18, 2) NULL,
    [Criteria5]         DECIMAL (18, 2) NULL,
    [Criteria6]         DECIMAL (18, 2) NULL,
    [Criteria7]         DECIMAL (18, 2) NULL,
    [Criteria8]         DECIMAL (18, 2) NULL,
    [Criteria9]         DECIMAL (18, 2) NULL,
    [Criteria10]        DECIMAL (18, 2) NULL,
    [Criteria11]        DECIMAL (18, 2) NULL,
    [Criteria12]        DECIMAL (18, 2) NULL,
    [Criteria13]        DECIMAL (18, 2) NULL,
    [Criteria14]        DECIMAL (18, 2) NULL,
    [Criteria15]        DECIMAL (18, 2) NULL,
    [Criteria16]        DECIMAL (18, 2) NULL,
    [Criteria17]        DECIMAL (18, 2) NULL,
    [SuspectionLevelId] INT             NULL,
    [StrategyId]        INT             NULL,
    [PeriodId]          INT             NULL,
    CONSTRAINT [PK_SubscriberThresholds] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SubscriberThresholds_Periods] FOREIGN KEY ([PeriodId]) REFERENCES [dbo].[Periods] ([Id]),
    CONSTRAINT [FK_SubscriberThresholds_Suspection_Level] FOREIGN KEY ([SuspectionLevelId]) REFERENCES [dbo].[Suspicion_Level] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Strategy]
    ON [dbo].[SubscriberThresholds]([SuspectionLevelId] ASC, [StrategyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateDay]
    ON [dbo].[SubscriberThresholds]([DateDay] ASC);

