CREATE TABLE [FraudAnalysis].[AccountStatus] (
    [AccountNumber] VARCHAR (50) NOT NULL,
    [Status]        INT          NOT NULL,
    CONSTRAINT [PK_AccountStatus] PRIMARY KEY CLUSTERED ([AccountNumber] ASC)
);

