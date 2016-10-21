CREATE TABLE [queue].[ExecutionFlow] (
    [Id]                           UNIQUEIDENTIFIER CONSTRAINT [DF__ExecutionFlo__Id__18AC8967] DEFAULT (newid()) NOT NULL,
    [OldId]                        INT              NULL,
    [Name]                         NVARCHAR (255)   NOT NULL,
    [ExecutionFlowDefinitionID]    UNIQUEIDENTIFIER NOT NULL,
    [OldExecutionFlowDefinitionID] INT              NULL,
    [timestamp]                    ROWVERSION       NULL,
    CONSTRAINT [pk_ExecutionFlow] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ExecutionFlow_ExecutionFlowDefinition] FOREIGN KEY ([ExecutionFlowDefinitionID]) REFERENCES [queue].[ExecutionFlowDefinition] ([Id])
);





