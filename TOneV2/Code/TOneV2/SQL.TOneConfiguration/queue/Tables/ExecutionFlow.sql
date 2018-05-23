CREATE TABLE [queue].[ExecutionFlow] (
    [Id]                        UNIQUEIDENTIFIER CONSTRAINT [DF__ExecutionFlo__Id__18AC8967] DEFAULT (newid()) NOT NULL,
    [Name]                      NVARCHAR (255)   NOT NULL,
    [ExecutionFlowDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [timestamp]                 ROWVERSION       NULL,
    [CreatedTime]               DATETIME         CONSTRAINT [DF_ExecutionFlow_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [pk_ExecutionFlow] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ExecutionFlow_ExecutionFlowDefinition] FOREIGN KEY ([ExecutionFlowDefinitionID]) REFERENCES [queue].[ExecutionFlowDefinition] ([Id])
);









