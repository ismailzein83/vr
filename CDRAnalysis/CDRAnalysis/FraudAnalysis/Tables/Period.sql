CREATE TABLE [FraudAnalysis].[Period] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NULL,
    CONSTRAINT [PK_Period] PRIMARY KEY CLUSTERED ([Id] ASC)
);



