CREATE TABLE [FraudAnalysis].[Aggregate] (
    [ID]                  INT          NOT NULL,
    [Name]                VARCHAR (50) NOT NULL,
    [OperatorTypeAllowed] INT          NOT NULL,
    [NumberPrecision]     VARCHAR (50) NULL,
    [timestamp]           ROWVERSION   NOT NULL,
    CONSTRAINT [PK_AggregateDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);

