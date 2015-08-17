CREATE TABLE [FraudAnalysis].[AccountCase] (
    [AccountNumber] VARCHAR (50) NOT NULL,
    [StatusID]      INT          NOT NULL,
    [ValidTill]     DATETIME     NULL,
    CONSTRAINT [PK_AccountCase] PRIMARY KEY CLUSTERED ([AccountNumber] ASC)
);

