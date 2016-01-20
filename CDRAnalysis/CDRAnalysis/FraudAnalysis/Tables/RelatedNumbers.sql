CREATE TABLE [FraudAnalysis].[RelatedNumbers] (
    [AccountNumber]  VARCHAR (30)  NOT NULL,
    [RelatedNumbers] VARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([AccountNumber] ASC)
);



