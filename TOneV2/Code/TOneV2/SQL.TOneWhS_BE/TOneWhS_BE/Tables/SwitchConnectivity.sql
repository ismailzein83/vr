CREATE TABLE [TOneWhS_BE].[SwitchConnectivity] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (450) NOT NULL,
    [CarrierAccountID] INT            NOT NULL,
    [SwitchID]         INT            NOT NULL,
    [Settings]         NVARCHAR (MAX) NOT NULL,
    [BED]              DATETIME       NOT NULL,
    [EED]              DATETIME       NULL,
    [CreatedTime]      DATETIME       CONSTRAINT [DF_SwitchConnectivity_CreatedTime] DEFAULT (getdate()) NULL,
    [SourceID]         VARCHAR (50)   NULL,
    [timestamp]        ROWVERSION     NULL,
    [CreatedBy]        INT            NULL,
    [LastModifiedBy]   INT            NULL,
    [LastModifiedTime] DATETIME       NULL,
    CONSTRAINT [PK_SwitchConnectivity] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SwitchConnectivity_CarrierAccount1] FOREIGN KEY ([CarrierAccountID]) REFERENCES [TOneWhS_BE].[CarrierAccount] ([ID]),
    CONSTRAINT [FK_SwitchConnectivity_Switch1] FOREIGN KEY ([SwitchID]) REFERENCES [TOneWhS_BE].[Switch] ([ID])
);







