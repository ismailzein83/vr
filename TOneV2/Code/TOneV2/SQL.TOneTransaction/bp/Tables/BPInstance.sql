CREATE TABLE [bp].[BPInstance] (
    [ID]                          BIGINT           IDENTITY (1, 1) NOT NULL,
    [Title]                       NVARCHAR (1000)  NULL,
    [ParentID]                    BIGINT           NULL,
    [DefinitionID]                UNIQUEIDENTIFIER NOT NULL,
    [OldDefinitionID]             INT              NULL,
    [ServiceInstanceID]           UNIQUEIDENTIFIER NULL,
    [InitiatorUserId]             INT              NOT NULL,
    [WorkflowInstanceID]          UNIQUEIDENTIFIER NULL,
    [InputArgument]               NVARCHAR (MAX)   NULL,
    [CompletionNotifier]          NVARCHAR (MAX)   NULL,
    [ExecutionStatus]             INT              NOT NULL,
    [LastMessage]                 NVARCHAR (MAX)   NULL,
    [EntityId]                    VARCHAR (255)    NULL,
    [ViewRequiredPermissionSetId] INT              NULL,
    [CreatedTime]                 DATETIME         CONSTRAINT [DF_BPInstance_CreatedTime] DEFAULT (getdate()) NULL,
    [StatusUpdatedTime]           DATETIME         NULL,
    [timestamp]                   ROWVERSION       NULL,
    CONSTRAINT [PK_BPInstance_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);
























GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_DefinitionID]
    ON [bp].[BPInstance]([OldDefinitionID] ASC);




GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_CreatedTime]
    ON [bp].[BPInstance]([CreatedTime] ASC);


GO



GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_ExecutionStatus]
    ON [bp].[BPInstance]([ExecutionStatus] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BPInstance_ViewReqPrmSet]
    ON [bp].[BPInstance]([ViewRequiredPermissionSetId] ASC);

