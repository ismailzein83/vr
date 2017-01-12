CREATE TABLE [TOneWhS_BE].[SwitchConnectivityZTemp] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (450) NOT NULL,
    [CarrierAccountID] INT            NOT NULL,
    [SwitchID]         INT            NOT NULL,
    [Settings]         NVARCHAR (MAX) NOT NULL,
    [BED]              DATETIME       NOT NULL,
    [EED]              DATETIME       NULL,
    [CreatedTime]      DATETIME       NULL,
    [SourceID]         VARCHAR (50)   NULL,
    [timestamp]        ROWVERSION     NULL
);

