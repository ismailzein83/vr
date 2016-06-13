﻿CREATE TABLE [TOneWhS_SPL].[SupplierZoneRate_Preview] (
    [ProcessInstanceID] BIGINT         NOT NULL,
    [CountryID]         INT            NOT NULL,
    [ZoneName]          NVARCHAR (255) NOT NULL,
    [RecentZoneName]    NVARCHAR (255) NULL,
    [ZoneChangeType]    INT            NOT NULL,
    [ZoneBED]           DATETIME       NOT NULL,
    [ZoneEED]           DATETIME       NULL,
    [CurrentRate]       DECIMAL (9, 5) NULL,
    [CurrentRateBED]    DATETIME       NULL,
    [CurrentRateEED]    DATETIME       NULL,
    [ImportedRate]      DECIMAL (9, 5) NULL,
    [ImportedRateBED]   DATETIME       NULL,
    [RateChangeType]    INT            NOT NULL
);

