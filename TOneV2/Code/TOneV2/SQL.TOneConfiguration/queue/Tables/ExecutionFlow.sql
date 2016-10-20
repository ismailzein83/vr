CREATE TABLE [queue].[ExecutionFlow] (
    [Name]                         NVARCHAR (255)   NOT NULL,
    [timestamp]                    ROWVERSION       NULL,
    [OldId]                        INT              NULL,
    [Id]                           UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [OldExecutionFlowDefinitionID] INT              NULL,
    [ExecutionFlowDefinitionID]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [pk_ExecutionFlow] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ExecutionFlow_ExecutionFlowDefinition] FOREIGN KEY ([ExecutionFlowDefinitionID]) REFERENCES [queue].[ExecutionFlowDefinition] ([Id])
);



