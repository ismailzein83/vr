CREATE TABLE [VR_AccountManager].[AccountManager] (
    [ID]                         BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountManagerDefinitionID] UNIQUEIDENTIFIER NOT NULL,
    [UserID]                     INT              NOT NULL,
    [Settings]                   NVARCHAR (MAX)   NULL,
    [CreatedTime]                DATETIME         CONSTRAINT [DF_AccountManager_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]                  ROWVERSION       NULL,
    CONSTRAINT [PK_AccountManager] PRIMARY KEY CLUSTERED ([ID] ASC)
);

