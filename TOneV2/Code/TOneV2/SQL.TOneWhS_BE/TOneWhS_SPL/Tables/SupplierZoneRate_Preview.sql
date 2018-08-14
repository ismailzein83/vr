CREATE TABLE [TOneWhS_SPL].[SupplierZoneRate_Preview] (
    [ProcessInstanceID]      BIGINT          NOT NULL,
    [CountryID]              INT             NOT NULL,
    [ZoneName]               NVARCHAR (255)  NOT NULL,
    [RecentZoneName]         NVARCHAR (255)  NULL,
    [ZoneChangeType]         INT             NOT NULL,
    [ZoneBED]                DATETIME        NOT NULL,
    [ZoneEED]                DATETIME        NULL,
    [SystemRate]             DECIMAL (20, 8) NULL,
    [SystemRateBED]          DATETIME        NULL,
    [SystemRateEED]          DATETIME        NULL,
    [ImportedRate]           DECIMAL (20, 8) NULL,
    [ImportedRateBED]        DATETIME        NULL,
    [RateChangeType]         INT             NOT NULL,
    [SystemZoneServiceIds]   VARCHAR (MAX)   NULL,
    [SystemZoneServiceBED]   DATETIME        NULL,
    [SystemZoneServiceEED]   DATETIME        NULL,
    [ImportedZoneServiceIds] VARCHAR (MAX)   NULL,
    [ImportedZoneServiceBED] DATETIME        NULL,
    [ZoneServiceChangeType]  INT             NOT NULL,
    [IsExcluded]             BIT             CONSTRAINT [DF_SupplierZoneRate_Preview_IsExcluded] DEFAULT ((0)) NULL
);












GO
CREATE CLUSTERED INDEX [IX_SupplierZoneRate_Preview_ProcessInstanceID]
    ON [TOneWhS_SPL].[SupplierZoneRate_Preview]([ProcessInstanceID] ASC);

