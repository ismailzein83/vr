CREATE TABLE [queue].[ExecutionFlowDefinition] (
    [Name]          VARCHAR (255)    NOT NULL,
    [Title]         NVARCHAR (255)   NOT NULL,
    [ExecutionTree] NVARCHAR (MAX)   NULL,
    [Stages]        NVARCHAR (MAX)   NULL,
    [timestamp]     ROWVERSION       NULL,
    [OldId]         INT              NULL,
    [Id]            UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    CONSTRAINT [pk_ExecutionFlowDefinition] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_ExecutionFlowType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);



