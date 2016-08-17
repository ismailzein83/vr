CREATE TABLE [TOneWhS_SPL].[SupplierZoneRate_Preview] (
    [ProcessInstanceID] BIGINT          NOT NULL,
    [CountryID]         INT             NOT NULL,
    [ZoneName]          NVARCHAR (255)  NOT NULL,
    [RecentZoneName]    NVARCHAR (255)  NULL,
    [ZoneChangeType]    INT             NOT NULL,
    [ZoneBED]           DATETIME        NOT NULL,
    [ZoneEED]           DATETIME        NULL,
    [SystemRate]        DECIMAL (20, 8) NULL,
    [SystemRateBED]     DATETIME        NULL,
    [SystemRateEED]     DATETIME        NULL,
    [ImportedRate]      DECIMAL (20, 8) NULL,
    [ImportedRateBED]   DATETIME        NULL,
    [RateChangeType]    INT             NOT NULL
);





