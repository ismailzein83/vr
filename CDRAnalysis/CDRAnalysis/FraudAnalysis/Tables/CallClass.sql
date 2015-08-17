CREATE TABLE [FraudAnalysis].[CallClass] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (255) NULL,
    [NetType]     INT           NULL,
    CONSTRAINT [PK_CallClass] PRIMARY KEY CLUSTERED ([Id] ASC)
);



