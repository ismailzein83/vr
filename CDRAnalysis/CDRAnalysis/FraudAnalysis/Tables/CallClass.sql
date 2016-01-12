CREATE TABLE [FraudAnalysis].[CallClass] (
    [ID]          INT           IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (255) NOT NULL,
    [NetType]     INT           NOT NULL,
    [timestamp]   ROWVERSION    NOT NULL,
    CONSTRAINT [PK_CallClass] PRIMARY KEY CLUSTERED ([ID] ASC)
);





