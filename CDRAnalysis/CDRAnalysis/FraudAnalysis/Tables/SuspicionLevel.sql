CREATE TABLE [FraudAnalysis].[SuspicionLevel] (
    [Id]   INT          IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (20) NULL,
    CONSTRAINT [PK_suspection_level] PRIMARY KEY CLUSTERED ([Id] ASC)
);

