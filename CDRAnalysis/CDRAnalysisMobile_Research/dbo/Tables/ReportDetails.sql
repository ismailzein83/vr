CREATE TABLE [dbo].[ReportDetails] (
    [Id]               INT          IDENTITY (1, 1) NOT NULL,
    [ReportId]         INT          NULL,
    [SubscriberNumber] VARCHAR (30) NULL,
    [StrategyId]       INT          NULL,
    CONSTRAINT [PK_ReportDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReportDetails_Report] FOREIGN KEY ([ReportId]) REFERENCES [dbo].[Report] ([Id]),
    CONSTRAINT [FK_ReportDetails_Strategy] FOREIGN KEY ([StrategyId]) REFERENCES [dbo].[Strategy] ([Id])
);

