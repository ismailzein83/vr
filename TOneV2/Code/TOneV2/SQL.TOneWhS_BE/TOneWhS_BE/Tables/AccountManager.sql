CREATE TABLE [TOneWhS_BE].[AccountManager] (
    [UserId]           INT        NULL,
    [CarrierAccountId] INT        NOT NULL,
    [RelationType]     INT        NOT NULL,
    [timestamp]        ROWVERSION NULL,
    [CreatedTime]      DATETIME   CONSTRAINT [DF_AccountManager_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_AccountManager] PRIMARY KEY CLUSTERED ([CarrierAccountId] ASC, [RelationType] ASC)
);



