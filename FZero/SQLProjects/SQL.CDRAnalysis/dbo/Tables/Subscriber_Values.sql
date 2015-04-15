CREATE TABLE [dbo].[Subscriber_Values] (
    [Id]               INT             IDENTITY (1, 1) NOT NULL,
    [SubscriberNumber] VARCHAR (50)    NULL,
    [FromDate]         DATETIME        NULL,
    [ToDate]           DATETIME        NULL,
    [CriteriaId]       INT             NULL,
    [StrategyId]       INT             NULL,
    [Value]            DECIMAL (18, 2) NULL,
    [PeriodId]         INT             NULL,
    CONSTRAINT [PK_Subscriber_Values] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Subscriber_Values_criteria_profile] FOREIGN KEY ([CriteriaId]) REFERENCES [dbo].[Criteria_Profile] ([Id]),
    CONSTRAINT [FK_Subscriber_Values_Periods] FOREIGN KEY ([PeriodId]) REFERENCES [dbo].[Periods] ([Id]),
    CONSTRAINT [FK_Subscriber_Values_Strategy] FOREIGN KEY ([StrategyId]) REFERENCES [dbo].[Strategy] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_FromDateToDAte]
    ON [dbo].[Subscriber_Values]([FromDate] ASC, [ToDate] ASC);

