CREATE TABLE [FraudAnalysis].[AccountStatus] (
    [AccountNumber] VARCHAR (50)   NOT NULL,
    [Status]        INT            NOT NULL,
    [AccountInfo]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AccountStatus] PRIMARY KEY CLUSTERED ([AccountNumber] ASC)
);






GO
CREATE NONCLUSTERED INDEX [IX_AccountStatus_Status]
    ON [FraudAnalysis].[AccountStatus]([Status] ASC);

