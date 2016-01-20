CREATE TABLE [FraudAnalysis].[AccountInfo] (
    [AccountNumber] VARCHAR (50)   NOT NULL,
    [InfoDetail]    NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AccountInfo] PRIMARY KEY CLUSTERED ([AccountNumber] ASC)
);

