CREATE TABLE [FraudAnalysis].[Filter] (
    [ID]                  INT             NOT NULL,
    [Abbreviation]        VARCHAR (50)    NOT NULL,
    [OperatorTypeAllowed] INT             NOT NULL,
    [Description]         VARCHAR (1000)  NOT NULL,
    [Label]               VARCHAR (50)    NOT NULL,
    [ToolTip]             VARCHAR (250)   NOT NULL,
    [ExcludeHourly]       BIT             NOT NULL,
    [CompareOperator]     INT             NOT NULL,
    [MinValue]            DECIMAL (18, 3) NOT NULL,
    [MaxValue]            DECIMAL (18, 3) NOT NULL,
    [DecimalPrecision]    INT             NOT NULL,
    [timestamp]           ROWVERSION      NOT NULL,
    CONSTRAINT [PK_FilterDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);

