CREATE TABLE [FraudAnalysis].[CaseStatus] (
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (255) NULL,
    CONSTRAINT [PK_CaseStatus] PRIMARY KEY CLUSTERED ([Id] ASC)
);

