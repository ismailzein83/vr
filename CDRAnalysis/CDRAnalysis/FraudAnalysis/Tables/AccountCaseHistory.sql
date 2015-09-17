CREATE TABLE [FraudAnalysis].[AccountCaseHistory] (
    [ID]         INT           IDENTITY (1, 1) NOT NULL,
    [CaseID]     INT           NOT NULL,
    [UserID]     INT           NULL,
    [Status]     INT           NOT NULL,
    [StatusTime] DATETIME      NOT NULL,
    [Reason]     VARCHAR (MAX) NULL,
    CONSTRAINT [PK_AccountCaseHistory] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_AccountCaseHistory_AccountCase1] FOREIGN KEY ([CaseID]) REFERENCES [FraudAnalysis].[AccountCase] ([ID]) ON DELETE CASCADE
);






GO
CREATE NONCLUSTERED INDEX [IX_AccountCaseHistory_CaseID]
    ON [FraudAnalysis].[AccountCaseHistory]([CaseID] ASC);

