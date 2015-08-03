CREATE TABLE [BEntity].[AccountManager] (
    [UserId]           INT         NOT NULL,
    [CarrierAccountId] VARCHAR (5) NOT NULL,
    [RelationType]     INT         NOT NULL,
    CONSTRAINT [PK_AccountManager] PRIMARY KEY CLUSTERED ([CarrierAccountId] ASC, [RelationType] ASC),
    CONSTRAINT [FK_AccountManager_CarrierAccount] FOREIGN KEY ([CarrierAccountId]) REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
);

