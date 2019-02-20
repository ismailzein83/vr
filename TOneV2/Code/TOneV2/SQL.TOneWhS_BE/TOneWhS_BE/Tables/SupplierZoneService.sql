CREATE TABLE [TOneWhS_BE].[SupplierZoneService] (
    [ID]                   BIGINT        NOT NULL,
    [ZoneID]               BIGINT        NULL,
    [PriceListID]          INT           NULL,
    [SupplierID]           INT           NOT NULL,
    [ReceivedServicesFlag] VARCHAR (MAX) NOT NULL,
    [EffectiveServiceFlag] VARCHAR (MAX) NULL,
    [BED]                  DATETIME      NOT NULL,
    [EED]                  DATETIME      NULL,
    [SourceID]             VARCHAR (50)  NULL,
    [timestamp]            ROWVERSION    NULL,
    [LastModifiedTime]     DATETIME      CONSTRAINT [DF_SupplierZoneService_LastModifiedTime] DEFAULT (getdate()) NULL,
    [CreatedTime]          DATETIME      CONSTRAINT [DF_SupplierZoneService_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_SupplierZoneService] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SupplierZoneService_SupplierZone] FOREIGN KEY ([ZoneID]) REFERENCES [TOneWhS_BE].[SupplierZone] ([ID])
);


















GO
CREATE NONCLUSTERED INDEX [IX_SupplierZoneService_timestamp]
    ON [TOneWhS_BE].[SupplierZoneService]([timestamp] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_SupplierZoneService_ZoneID]
    ON [TOneWhS_BE].[SupplierZoneService]([ZoneID] ASC);

