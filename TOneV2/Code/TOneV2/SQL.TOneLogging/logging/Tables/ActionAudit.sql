CREATE TABLE [logging].[ActionAudit] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserID]            INT            NULL,
    [URLID]             INT            NULL,
    [ModuleID]          INT            NULL,
    [EntityID]          INT            NULL,
    [ActionID]          INT            NULL,
    [ObjectID]          VARCHAR (255)  NULL,
    [ObjectName]        NVARCHAR (900) NULL,
    [ObjectTrackingID]  BIGINT         NULL,
    [ActionDescription] NVARCHAR (MAX) NULL,
    [LogTime]           DATETIME       NULL,
    [CreatedTime]       DATETIME       CONSTRAINT [DF_ActionAudit_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]         ROWVERSION     NULL,
    CONSTRAINT [IX_ActionAudit_ID] UNIQUE NONCLUSTERED ([ID] ASC)
);






GO
CREATE CLUSTERED INDEX [IX_ActionAudit_LogTime]
    ON [logging].[ActionAudit]([LogTime] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ActionAudit_UserID]
    ON [logging].[ActionAudit]([UserID] ASC);

