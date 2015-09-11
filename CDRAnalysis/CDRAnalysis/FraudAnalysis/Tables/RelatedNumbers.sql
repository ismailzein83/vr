CREATE TABLE [FraudAnalysis].[RelatedNumbers] (
    [AccountNumber]  VARCHAR (30)  NOT NULL,
    [RelatedNumbers] VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_RelatedNumbers] PRIMARY KEY CLUSTERED ([AccountNumber] ASC)
);

