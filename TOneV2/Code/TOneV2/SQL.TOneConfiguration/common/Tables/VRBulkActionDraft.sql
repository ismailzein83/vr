CREATE TABLE [common].[VRBulkActionDraft] (
    [ID]                        BIGINT           IDENTITY (1, 1) NOT NULL,
    [ItemID]                    VARCHAR (255)    NULL,
    [BulkActionDraftIdentifier] UNIQUEIDENTIFIER NULL,
    [CreatedTime]               DATETIME         CONSTRAINT [DF_VRBulkActionDraft_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_VRBulkActionDraft] PRIMARY KEY CLUSTERED ([ID] ASC)
);

