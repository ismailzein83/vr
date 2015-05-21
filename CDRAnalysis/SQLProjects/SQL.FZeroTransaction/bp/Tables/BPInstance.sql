CREATE TABLE [bp].[BPInstance] (
    [ID]                 BIGINT           IDENTITY (1, 1) NOT NULL,
    [Title]              NVARCHAR (1000)  NULL,
    [ParentID]           BIGINT           NULL,
    [DefinitionID]       INT              NOT NULL,
    [WorkflowInstanceID] UNIQUEIDENTIFIER NULL,
    [InputArgument]      NVARCHAR (MAX)   NULL,
    [ExecutionStatus]    INT              NOT NULL,
    [LockedByProcessID]  INT              NULL,
    [LastMessage]        NVARCHAR (MAX)   NULL,
    [RetryCount]         INT              NULL,
    [CreatedTime]        DATETIME         CONSTRAINT [DF_BPInstance_CreatedTime] DEFAULT (getdate()) NULL,
    [StatusUpdatedTime]  DATETIME         NULL,
    CONSTRAINT [PK_BPInstance_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_BPInstance_BPDefinition] FOREIGN KEY ([DefinitionID]) REFERENCES [bp].[BPDefinition] ([ID])
);




GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_ExecutionStatus]
    ON [bp].[BPInstance]([ExecutionStatus] ASC);


GO



GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_CreatedTime]
    ON [bp].[BPInstance]([CreatedTime] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_DefinitionID]
    ON [bp].[BPInstance]([DefinitionID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_LockedByProcessID]
    ON [bp].[BPInstance]([LockedByProcessID] ASC);

