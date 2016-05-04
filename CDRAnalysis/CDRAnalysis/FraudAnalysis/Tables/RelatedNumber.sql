CREATE TABLE [FraudAnalysis].[RelatedNumber] (
    [AccountNumber]        VARCHAR (30) NOT NULL,
    [RelatedAccountNumber] VARCHAR (30) NOT NULL,
    CONSTRAINT [PK_FraudAnalysis] PRIMARY KEY CLUSTERED ([AccountNumber] ASC, [RelatedAccountNumber] ASC)
);

