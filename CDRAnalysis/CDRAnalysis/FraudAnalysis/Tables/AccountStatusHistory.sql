CREATE TABLE [FraudAnalysis].[AccountStatusHistory] (
    [ID]            INT          IDENTITY (1, 1) NOT NULL,
    [AccountNumber] VARCHAR (50) NOT NULL,
    [UserID]        INT          NULL,
    [Status]        INT          NOT NULL,
    [StatusTime]    DATETIME     NULL,
    CONSTRAINT [PK_AccountStatusHistory] PRIMARY KEY CLUSTERED ([ID] ASC)
);

