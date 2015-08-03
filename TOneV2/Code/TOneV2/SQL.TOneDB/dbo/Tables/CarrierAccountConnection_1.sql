CREATE TABLE [dbo].[CarrierAccountConnection] (
    [ID]               INT          IDENTITY (1, 1) NOT NULL,
    [SwitchID]         TINYINT      NULL,
    [CarrierAccountID] VARCHAR (5)  NULL,
    [ConnectionType]   VARCHAR (20) NULL,
    [TAG]              VARCHAR (20) NULL,
    [Value]            VARCHAR (30) NULL,
    [UserID]           INT          NULL,
    [timestamp]        ROWVERSION   NULL,
    [GateWay]          VARCHAR (30) NULL,
    CONSTRAINT [PK_CarrierAccountConnection] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CarrierAccountConnection_CarrierAccount] FOREIGN KEY ([CarrierAccountID]) REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID]) ON DELETE CASCADE,
    CONSTRAINT [FK_CarrierAccountConnection_Switch] FOREIGN KEY ([SwitchID]) REFERENCES [dbo].[Switch] ([SwitchID]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CarrierAccountConnection_Switch]
    ON [dbo].[CarrierAccountConnection]([SwitchID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CarrierAccountConnection_Account]
    ON [dbo].[CarrierAccountConnection]([CarrierAccountID] ASC);

