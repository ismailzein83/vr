CREATE TABLE [TOneWhS_SMSBE].[ProcessDraft] (
    [ID]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [ProcessType]      INT            NOT NULL,
    [EntityID]         VARCHAR (50)   NOT NULL,
    [Changes]          NVARCHAR (MAX) NOT NULL,
    [Status]           INT            NOT NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_ProcessDraft_CreatedTime] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]        INT            NOT NULL,
    [LastModifiedTime] DATETIME       NOT NULL,
    [LastModifiedBy]   INT            NOT NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK_ProcessDraft] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ProcessDraft_ProcessType_EntityId]
    ON [TOneWhS_SMSBE].[ProcessDraft]([ProcessType] ASC, [EntityID] ASC);

