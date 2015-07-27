CREATE TABLE [FraudAnalysis].[SubscriberCase] (
    [SubscriberNumber] VARCHAR (50) NOT NULL,
    [StatusID]         INT          NOT NULL,
    [ValidTill]        DATETIME     NULL,
    CONSTRAINT [PK_SubscriberCase] PRIMARY KEY CLUSTERED ([SubscriberNumber] ASC),
    CONSTRAINT [FK_SubscriberCase_CaseStatus] FOREIGN KEY ([StatusID]) REFERENCES [FraudAnalysis].[CaseStatus] ([Id])
);



