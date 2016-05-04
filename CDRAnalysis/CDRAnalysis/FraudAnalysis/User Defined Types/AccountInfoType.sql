CREATE TYPE [FraudAnalysis].[AccountInfoType] AS TABLE (
    [AccountNumber] VARCHAR (50)   NOT NULL,
    [InfoDetail]    NVARCHAR (MAX) NULL);

