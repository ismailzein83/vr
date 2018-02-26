CREATE TABLE [VRNotification].[VRNotification] (
    [ID]                   BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserID]               INT              NOT NULL,
    [TypeID]               UNIQUEIDENTIFIER NOT NULL,
    [ParentType1]          VARCHAR (255)    NULL,
    [ParentType2]          VARCHAR (255)    NULL,
    [EventKey]             NVARCHAR (900)   NOT NULL,
    [ExecuteBPInstanceID]  BIGINT           NULL,
    [ClearBPInstanceID]    BIGINT           NULL,
    [Status]               INT              NOT NULL,
    [AlertLevelID]         UNIQUEIDENTIFIER NULL,
    [Description]          NVARCHAR (900)   NULL,
    [ErrorMessage]         NVARCHAR (MAX)   NULL,
    [EventPayload]         NVARCHAR (MAX)   NOT NULL,
    [RollbackEventPayload] NVARCHAR (MAX)   NULL,
    [Data]                 NVARCHAR (MAX)   NULL,
    [CreationTime]         DATETIME         CONSTRAINT [DF_VRNotification_CreationTime] DEFAULT (getdate()) NULL,
    [timestamp]            ROWVERSION       NULL,
    CONSTRAINT [PK_VRActiveNotification_1] PRIMARY KEY CLUSTERED ([ID] ASC)
);
















GO
CREATE NONCLUSTERED INDEX [IDX_Notification_NotCleared]
    ON [VRNotification].[VRNotification]([TypeID] ASC, [ParentType1] ASC, [ParentType2] ASC, [CreationTime] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_VRNotification_Type]
    ON [VRNotification].[VRNotification]([TypeID] ASC);

