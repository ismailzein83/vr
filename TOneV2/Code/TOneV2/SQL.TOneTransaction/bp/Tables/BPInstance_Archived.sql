CREATE TABLE [bp].[BPInstance_Archived] (
    [ID]                          BIGINT           NOT NULL,
    [Title]                       NVARCHAR (1000)  NULL,
    [ParentID]                    BIGINT           NULL,
    [DefinitionID]                UNIQUEIDENTIFIER NOT NULL,
    [ServiceInstanceID]           UNIQUEIDENTIFIER NULL,
    [InitiatorUserId]             INT              NOT NULL,
    [WorkflowInstanceID]          UNIQUEIDENTIFIER NULL,
    [InputArgument]               NVARCHAR (MAX)   NULL,
    [CompletionNotifier]          NVARCHAR (MAX)   NULL,
    [ExecutionStatus]             INT              NOT NULL,
    [AssignmentStatus]            INT              NULL,
    [LastMessage]                 NVARCHAR (MAX)   NULL,
    [EntityId]                    VARCHAR (255)    NULL,
    [ViewRequiredPermissionSetId] INT              NULL,
    [CancellationRequestUserId]   INT              NULL,
    [CreatedTime]                 DATETIME         CONSTRAINT [DF_BPInstance_Archived_CreatedTime] DEFAULT (getdate()) NULL,
    [StatusUpdatedTime]           DATETIME         NULL,
    [TaskId]                      UNIQUEIDENTIFIER NULL,
    [timestamp]                   ROWVERSION       NULL,
    CONSTRAINT [PK_BPInstance_Archived_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_Archived_ViewReqPrmSet]
    ON [bp].[BPInstance_Archived]([ViewRequiredPermissionSetId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_Archived_ExecutionStatus]
    ON [bp].[BPInstance_Archived]([ExecutionStatus] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_Archived_DefinitionID]
    ON [bp].[BPInstance_Archived]([DefinitionID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_Archived_CreatedTime]
    ON [bp].[BPInstance_Archived]([CreatedTime] ASC);

