CREATE TABLE [TOneWhS_AM].[AccountManager] (
    [ID]               INT        IDENTITY (1, 1) NOT NULL,
    [UserId]           INT        NULL,
    [CreatedTime]      DATETIME   CONSTRAINT [DF_AccountManager_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT        NULL,
    [LastModifiedTime] DATETIME   NULL,
    [LastModifiedBy]   INT        NULL,
    [timestamp]        ROWVERSION NULL,
    [ParentId]         INT        NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

