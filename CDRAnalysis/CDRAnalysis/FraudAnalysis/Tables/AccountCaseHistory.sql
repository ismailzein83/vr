CREATE TABLE [FraudAnalysis].[AccountCaseHistory] (
    [ID]         INT      IDENTITY (1, 1) NOT NULL,
    [CaseID]     INT      NOT NULL,
    [UserID]     INT      NOT NULL,
    [Status]     INT      NOT NULL,
    [StatusTime] DATETIME NOT NULL,
    CONSTRAINT [PK_AccountCaseHistory] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_AccountCaseHistory_AccountCase1] FOREIGN KEY ([CaseID]) REFERENCES [FraudAnalysis].[AccountCase] ([ID])
);

