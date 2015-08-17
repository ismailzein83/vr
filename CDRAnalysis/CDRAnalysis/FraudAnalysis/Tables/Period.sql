CREATE TABLE [FraudAnalysis].[Period] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NULL,
    CONSTRAINT [PK_Periods] PRIMARY KEY CLUSTERED ([Id] ASC)
);

