CREATE TABLE [FraudAnalysis].[AccountCase] (
    [ID]               INT          IDENTITY (1, 1) NOT NULL,
    [AccountNumber]    VARCHAR (50) NOT NULL,
    [StatusID]         INT          NOT NULL,
    [ValidTill]        DATETIME     NULL,
    [StrategyId]       INT          NULL,
    [UserId]           INT          NULL,
    [LogDate]          DATETIME     CONSTRAINT [DF_AccountCase_LogDate] DEFAULT (getdate()) NOT NULL,
    [SuspicionLevelID] INT          NULL
);








GO
CREATE CLUSTERED INDEX [IX_AccountCase_AccountNumber]
    ON [FraudAnalysis].[AccountCase]([AccountNumber] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AccountCase_LogDate]
    ON [FraudAnalysis].[AccountCase]([LogDate] ASC);

