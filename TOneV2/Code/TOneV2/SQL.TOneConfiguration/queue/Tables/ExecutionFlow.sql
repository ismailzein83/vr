CREATE TABLE [queue].[ExecutionFlow] (
    [ID]                        INT            IDENTITY (1, 1) NOT NULL,
    [Name]                      NVARCHAR (255) NOT NULL,
    [ExecutionFlowDefinitionID] INT            NOT NULL,
    [timestamp]                 ROWVERSION     NULL,
    CONSTRAINT [PK_ExecutionFlow] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ExecutionFlow_ExecutionFlowDefinition] FOREIGN KEY ([ExecutionFlowDefinitionID]) REFERENCES [queue].[ExecutionFlowDefinition] ([ID])
);

