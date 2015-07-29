CREATE TABLE [queue].[ExecutionFlowDefinition] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (255)  NOT NULL,
    [Title]         NVARCHAR (255) NOT NULL,
    [ExecutionTree] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_ExecutionFlowType] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_ExecutionFlowType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

