CREATE TABLE [TOneWhS_BE].[CustomerZone] (
    [ID]                 INT           NOT NULL,
    [CustomerID]         INT           NOT NULL,
    [SelledZoneSettings] VARCHAR (MAX) NOT NULL,
    [BED]                DATETIME      NOT NULL,
    [EED]                DATETIME      NULL,
    CONSTRAINT [PK_CustomerZones] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CustomerZones_CarrierAccount] FOREIGN KEY ([CustomerID]) REFERENCES [TOneWhS_BE].[CarrierAccount] ([ID])
);

