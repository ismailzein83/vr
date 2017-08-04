CREATE TABLE [FraudAnalysis].[AccountStatus] (
    [AccountNumber] VARCHAR (50)  NOT NULL,
    [Status]        INT           NOT NULL,
    [ValidTill]     DATETIME      NULL,
    [Source]        INT           NOT NULL,
    [Reason]        VARCHAR (MAX) NULL,
    [UserID]        INT           NOT NULL,
    [LastUpdatedOn] DATETIME      NULL,
    CONSTRAINT [PK_AccountStatus] PRIMARY KEY CLUSTERED ([AccountNumber] ASC)
);














GO
CREATE NONCLUSTERED INDEX [IX_AccountStatus_Status]
    ON [FraudAnalysis].[AccountStatus]([Status] ASC);

